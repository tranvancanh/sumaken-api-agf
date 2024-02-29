using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class QrcodeModel
    {
        public class QrcodeItem
        {
            /// <summary>
            /// 0:納期
            /// </summary>
            public string DeliveryDate { get; set; } = string.Empty;
            public string DeleveryDate { get; set; } = string.Empty;
            /// <summary>
            /// 1:便
            /// </summary>
            public string DeliveryTimeClass { get; set; } = string.Empty;
            /// <summary>
            /// 2:データ区分
            /// </summary>
            public string DataClass { get; set; } = string.Empty;
            /// <summary>
            /// 3:発注区分
            /// </summary>
            public string OrderClass { get; set; } = string.Empty;
            /// <summary>
            /// 4:伝票番号
            /// </summary>
            public string DeliverySlipNumber { get; set; } = string.Empty;
            /// <summary>
            /// 5:仕入先コード
            /// </summary>
            public string SupplierCode { get; set; } = string.Empty;
            /// <summary>
            /// 6:仕入先区分
            /// </summary>
            public string SupplierClass { get; set; } = string.Empty;
            /// <summary>
            /// 7:製品コード
            /// </summary>
            public string ProductCode { get; set; } = string.Empty;
            /// <summary>
            /// 8:製品略称（略番、背番）
            /// </summary>
            public string ProductAbbreviation { get; set; } = string.Empty;
            /// <summary>
            /// 9:発行枝番（シリアル）
            /// </summary>
            public int ProductLabelBranchNumber { get; set; } 
            /// <summary>
            /// 10:数量
            /// </summary>
            public int Quantity { get; set; } 
            /// <summary>
            /// 11:次工程1（納入先1）
            /// </summary>
            public string NextProcess1 { get; set; } = string.Empty;
            /// <summary>
            /// 12:置き場1（受入1）
            /// </summary>
            public string Location1 { get; set; } = string.Empty;
            /// <summary>
            /// 13:次工程2（納入先2）
            /// </summary>
            public string NextProcess2 { get; set; } = string.Empty;
            /// <summary>
            /// 14:箱種（荷姿）
            /// </summary>
            public string Packing { get; set; } = string.Empty;
            /// <summary>
            /// 15:客先コード
            /// </summary>
            public string CustomerCode { get; set; } = string.Empty;
            /// <summary>
            /// 16:客先区分
            /// </summary>
            public string CustomerClass { get; set; } = string.Empty;
            /// <summary>
            /// 17:客先製品コード
            /// </summary>
            public string CustomerProductCode { get; set; } = string.Empty;
            /// <summary>
            /// 18:客先製品略称（略番、背番）
            /// </summary>
            public string CustomerProductAbbreviation { get; set; } = string.Empty;
            /// <summary>
            /// 19:客先発行枝番（シリアル）
            /// </summary>
            public int CustomerProductLabelBranchNumber { get; set; }
            /// <summary>
            /// 14:置き場2（受入2）
            /// </summary>
            public string Location2 { get; set; } = string.Empty;

            public int ProductLabelBranchNumber2 { get; set; }  // 出荷枝番
            public bool StateFlag { get; set; }  // 出荷データのみ

            public static implicit operator QrcodeItem(List<ReceiveModel.RegistDataRecord> v)
            {
                throw new NotImplementedException();
            }
        }

        public class M_QrcodeIndex
        {
            public string IdentifyString { get; set; }
            public int IdentifyIndex { get; set; }

            public string IdentifySecondString { get; set; }
            public int IdentifySecondIndex { get; set; }

            public int MaxStringLength { get; set; }

            public bool ToyotaEdiWgFlag { get; set; }

            public int DeleveryDateIndex { get; set; }
            public int DeliveryTimeClassIndex { get; set; }
            public int DataClassIndex { get; set; }
            public int OrderClassIndex { get; set; }
            public int DeliverySlipNumberIndex { get; set; }
            public int SupplierCodeIndex { get; set; }
            public int SupplierClassIndex { get; set; }
            public int ProductCodeIndex { get; set; }
            public int ProductAbbreviationIndex { get; set; }
            public int ProductLabelBranchNumberIndex { get; set; }
            public int QuantityIndex { get; set; }
            public int NextProcess1Index { get; set; }
            public int NextProcess2Index { get; set; }
            public int Location1Index { get; set; }
            public int Location2ndex { get; set; }
            public int PackingIndex { get; set; }

            public int DeleveryDateLength { get; set; }
            public int DeliveryTimeClassLength { get; set; }
            public int DataClassLength { get; set; }
            public int OrderClassLength { get; set; }
            public int DeliverySlipNumberLength { get; set; }
            public int SupplierCodeLength { get; set; }
            public int SupplierClassLength { get; set; }
            public int ProductCodeLength { get; set; }
            public int ProductAbbreviationLength { get; set; }
            public int ProductLabelBranchNumberLength { get; set; }
            public int QuantityLength { get; set; }
            public int NextProcess1Length { get; set; }
            public int NextProcess2Length { get; set; }
            public int Location1Length { get; set; }
            public int Location2Length { get; set; }
            public int PackingLength { get; set; }
            public bool ForShipmentFlag {  get; set; }

        }

        public static List<M_QrcodeIndex> GetQrcodeIndex(string databaseName, int depoID, int handyPageID)
        {
            var qrcodeIndices = new List<M_QrcodeIndex>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT 
                                               IdentifyString
                                              ,IdentifyIndex
                                              ,IdentifySecondString
                                              ,IdentifySecondIndex
                                              ,DeleveryDateIndex
                                              ,DeleveryDateLength
                                              ,DeliveryTimeClassIndex
                                              ,DeliveryTimeClassLength
                                              ,DataClassIndex
                                              ,DataClassLength
                                              ,OrderClassIndex
                                              ,OrderClassLength
                                              ,DeliverySlipNumberIndex
                                              ,DeliverySlipNumberLength
                                              ,SupplierCodeIndex
                                              ,SupplierCodeLength
                                              ,SupplierClassIndex
                                              ,SupplierClassLength
                                              ,NextProcess1Index
                                              ,NextProcess1Length
                                              ,Location1Index
                                              ,Location1Length
                                              ,NextProcess2Index
                                              ,NextProcess2Length
                                              ,Location2Index
                                              ,Location2Length
                                              ,ProductCodeIndex
                                              ,ProductCodeLength
                                              ,ProductLabelBranchNumberIndex
                                              ,ProductLabelBranchNumberLength
                                              ,ProductAbbreviationIndex
                                              ,ProductAbbreviationLength
                                              ,QuantityIndex
                                              ,QuantityLength
                                              ,PackingIndex
                                              ,PackingLength
                                              ,MaxStringLength
                                              ,ToyotaEdiWgFlag
                                              ,ForShipmentFlag
                                          FROM M_Qrcode AS A
                                          LEFT OUTER JOIN M_QrcodeIndex AS B ON A.QrcodeIndexID = B.QrcodeIndexID
                                          WHERE 1=1
                                              AND A.DepoID = @DepoID
                                              AND A.HandyPageID = @HandyPageID
                                                ";
                    var param = new
                    {
                        DepoID = depoID,
                        HandyPageID = handyPageID
                    };
                    qrcodeIndices = connection.Query<M_QrcodeIndex>(query, param).ToList();

                    return qrcodeIndices;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}
