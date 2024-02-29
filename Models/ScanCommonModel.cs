using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static SakaguraAGFWebApi.Models.QrcodeModel;
using static SakaguraAGFWebApi.Models.ReceiveModel;

namespace SakaguraAGFWebApi.Models
{
    public class ScanCommonModel
    {
        public class ScanPostBody
        {
            [Required]
            public int DepoID { get; set; }
            [Required]
            public int HandyPageID { get; set; }
            [Required]
            public int HandyOperationClass { get; set; }

            public string HandyOperationMessage { get; set; } = String.Empty;

            public string ScanString1 { get; set; } = String.Empty;
            public string ScanString2 { get; set; } = String.Empty;
            public string ScanChangeData { get; set; } = String.Empty;
            public string ScanChangeData2 { get; set; } = String.Empty;

            [Required]
            public string ProcessDate { get; set; } = String.Empty;
            public string DuplicateCheckStartProcessDate { get; set; } = String.Empty;
            [Required]
            public int HandyUserID { get; set; }
            [Required]
            public string Device { get; set; } = String.Empty;
            [Required]
            public bool StoreInFlag { get; set; }

            public string ScanStoreAddress1 { get; set; } = String.Empty;
            public string ScanStoreAddress2 { get; set; } = String.Empty;
            public int InputQuantity { get; set; }
            public int InputPackingCount { get; set; }

            [Required]
            public DateTime ScanTime { get; set; }
            [Required]
            public double Latitude { get; set; }
            [Required]
            public double Longitude { get; set; }
        }

        public static (bool result, List<RegistDataRecord> registDatas) GetScanRegistData(List<ScanPostBody> bodies)
        {
            bool result = true;
            var registDatas = new List<RegistDataRecord>();

            try
            {

                for (int j = 0; j <= bodies.Count - 1; j++)
                {
                    var registData = new RegistDataRecord();
                    var qrcodeItem = new QrcodeItem();

                    if (bodies[j].HandyOperationClass != 0)
                    {
                        // OK以外の場合
                        registData.PostBody = bodies[j];
                        registData.QrcodeItem = qrcodeItem;
                    }
                    else
                    {
                        // パッケージ現品票QR型の値を取得して変換
                        string[] items = bodies[j].ScanChangeData.Split(':');

                        // 納期
                        var deliveryDateString = items[0];
                        if (deliveryDateString == "")
                        {
                            qrcodeItem.DeliveryDate = "";
                        }
                        else if (DateTime.TryParse(deliveryDateString, out DateTime deliveryDate))
                        {
                            qrcodeItem.DeliveryDate = deliveryDate.ToString("yyyy/MM/dd");
                        }
                        else
                        {
                            result = false;
                            break;
                        }

                        // 便
                        qrcodeItem.DeliveryTimeClass = items[1];

                        // データ区分
                        qrcodeItem.DataClass = items[2];

                        // 発注区分
                        qrcodeItem.OrderClass = items[3];

                        // 伝票番号
                        qrcodeItem.DeliverySlipNumber = items[4];

                        // 仕入先コード
                        qrcodeItem.SupplierCode = items[5];

                        // 仕入先区分
                        qrcodeItem.SupplierClass = items[6];

                        // 製品コード
                        qrcodeItem.ProductCode = items[7];

                        // 製品略称
                        qrcodeItem.ProductAbbreviation = items[8];

                        // 発行枝番（シリアル）
                        int branchNumber;
                        if (!int.TryParse(items[9], out branchNumber))
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            qrcodeItem.ProductLabelBranchNumber = branchNumber;
                        }

                        // 数量
                        int quantity;
                        if (!int.TryParse(items[10], out quantity))
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            qrcodeItem.Quantity = quantity;
                        }

                        // 次工程1（納入先1）
                        qrcodeItem.NextProcess1 = items[11];

                        // 置き場1（受入1）
                        qrcodeItem.Location1 = items[12];

                        // 次工程2（納入先2）
                        qrcodeItem.NextProcess2 = items[13];

                        // 箱種（荷姿）
                        qrcodeItem.Packing = items[14];

                        // 置き場2（受入2）
                        qrcodeItem.Location2 = items[15];

                        // 出荷枝番
                        qrcodeItem.ProductLabelBranchNumber2 = Convert.ToInt32(items[16]);

                        // 出荷かんばん<->製品かんばんプラグ
                        qrcodeItem.StateFlag = Convert.ToBoolean(items[17]);

                        registData.PostBody = bodies[j];
                        registData.QrcodeItem = qrcodeItem;
                        registData.QrcodeItem2 = JsonConvert.DeserializeObject<QrcodeItem>(bodies[j].ScanChangeData2);
                    }

                    registDatas.Add(registData);

                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return (result, registDatas);
        }
    }
}
