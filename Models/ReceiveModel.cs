using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using static SakaguraAGFWebApi.Models.QrcodeModel;
using static SakaguraAGFWebApi.Models.ScanCommonModel;

namespace SakaguraAGFWebApi.Models
{
    public class ReceiveModel
    {
        public class RegistData
        {
            public string DatabaseName { get; set; } = String.Empty;
            public DateTime CreateDate { get; set; }
            public List<RegistDataRecord> RegistDataRecords { get; set; } = new List<RegistDataRecord>();
        }

        public class RegistDataRecord
        {
            public ScanPostBody PostBody { get; set; } = new ScanPostBody();
            public QrcodeItem QrcodeItem { get; set; } = new QrcodeItem();
            public QrcodeItem QrcodeItem2 { get; set; } = new QrcodeItem(); // 出荷かんばん
        }

        public class D_Receive
        {
            public string SupplierCode { get; set; } = String.Empty;
            public string SupplierClass { get; set; } = String.Empty;
            public string ProductCode { get; set; } = String.Empty;
            public int ProductLabelBranchNumber { get; set; }
            public int Quantity { get; set; }
            public string Packing { get; set; } = String.Empty;
            public string NextProcess1 { get; set; } = String.Empty;
            public string NextProcess2 { get; set; } = String.Empty;
        }

        public static List<D_Receive> GetReceiveByReceiveDate(string databaseName, int depoID, string receiveDateStart, string receiveDateEnd)
        {
            var receives = new List<D_Receive>();

            try
            {
                var connectionString = new GetConnectString(databaseName).ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = @$"
                                        SELECT
                                               SupplierCode
                                              ,SupplierClass
                                              ,ReceiveDate
                                              ,ProductCode
                                              ,NextProcess1
                                              ,NextProcess2
                                              ,LotQuantity AS Quantity
                                              ,Packing
                                              ,PackingCount
                                              ,ProductLabelBranchNumber
                                          FROM D_Receive
                                          WHERE (1=1)
                                                AND DepoID = @DepoID
                                                AND ReceiveDate >= @ReceiveDateStart
                                                AND ReceiveDate <= @ReceiveDateEnd
                                                AND DeleteFlag = @DeleteFlag
                                                ";

                    var param = new
                    {
                        DepoID = depoID,
                        ReceiveDateStart = receiveDateStart,
                        ReceiveDateEnd = receiveDateEnd,
                        DeleteFlag = 0
                    };

                    receives = connection.Query<D_Receive>(query, param).ToList();

                    return receives;

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public class ReceivePostBackBody
        {
            /// <summary>
            /// 登録成功データカウント
            /// </summary>
            public int SuccessDataCount { get; set; }
            /// <summary>
            /// 既に登録済のデータカウント
            /// </summary>
            public int AlreadyRegisteredDataCount { get; set; }
            /// <summary>
            /// 既に登録済のデータ
            /// </summary>
            public List<QrcodeItem> AlreadyRegisteredDatas { get; set; } = new List<QrcodeItem>();
        }

        //public class AlreadyRegisteredData
        //{
        //    public string RegisteredProductCode { get; set; } = String.Empty;
        //    public string RegisteredSupplierCode { get; set; } = String.Empty;
        //    public int RegisteredQuantity { get; set; }
        //    public int RegisteredProductLabelBranchNumber { get; set; }
        //}

    }

}
