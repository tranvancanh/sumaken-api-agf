using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using static SakaguraAGFWebApi.Models.HandyReportLogModel;
using static SakaguraAGFWebApi.Models.QrcodeModel;
using static SakaguraAGFWebApi.Models.ReceiveModel;
using static SakaguraAGFWebApi.Models.ScanCommonModel;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private static int defaultInt = 0;
        private static long defaultBigInt = 0L;
        private static bool defaultBool = false;
        private static string defaultString = string.Empty;
        private static DateTime defaultDateTime = new DateTime(1900, 01, 01);

        private readonly ILogger<ShipmentController> _logger;
        public ShipmentController(ILogger<ShipmentController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Nlog is started to ShipmentController");
        }


        [HttpGet("{companyID}")]
        public async Task<IActionResult> Get(int companyID, int depoID, string receiveDateStart, string receiveDateEnd)
        {
            var registedData = new List<D_ShipmentModel>();
            try
            {
                var startTime = DateTime.Now;
                _logger.LogInformation("データ取得処理は開始");
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1)
                {
                    _logger.LogError("会社情報の取得に失敗しました");
                    return Responce.ExBadRequest("会社情報の取得に失敗しました"); 
                }
                var databaseName = companys[0].DatabaseName;
                if (string.IsNullOrEmpty(databaseName)) return Responce.ExBadRequest("データベースの取得に失敗しました");
                var connectionString = new GetConnectString(databaseName).ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    registedData = await GetShippedDataByDeliveryDate(databaseName, depoID, receiveDateStart, receiveDateEnd, connection);
                }
                var endTime = DateTime.Now;
                var elapsed = endTime - startTime;
                var completeTime = elapsed.ToString(@"hh\:mm\:ss\.ffff");
                _logger.LogInformation("データ取得処理は正常終了");
                _logger.LogInformation("データ取得時間は: " + completeTime);
            }
            catch (Exception ex)
            {
                _logger.LogError("データ取得処理は異常終了");
                _logger.LogError("Message   ：   " + ex.Message);
                return Responce.ExServerError(ex);
            }

            return Ok(registedData);
        }

        private async Task<List<D_ShipmentModel>> GetShippedDataByDeliveryDate(string databaseName, int depoID, string shipmentDateStart, string shipmentDateStop, SqlConnection connection = null, SqlTransaction transaction = null )
        {
            var registedData = new List<D_ShipmentModel>();
            var query = @$"
                        SELECT
                            [ShipmentID]
                            ,[ScanRecordID]
                            ,[ShipmentInstructionDetailID]
                            ,[StoreOutID]
                            ,[HandyMatchClass]
                            ,[HandyMatchResult]
                            ,[ShipmentDate]
                            ,[DepoID]
                            ,[DeliveryDate]
                            ,[DeliveryTimeClass]
                            ,[DeliverySlipNumber]
                            ,[DeliverySlipRowNumber]
                            ,[SupplierCode]
                            ,[SupplierClass]
                            ,[ProductCode]
                            ,[ProductAbbreviation]
                            ,[ProductManagementClass]
                            ,[ProductLabelBranchNumber]
                            ,[NextProcess1]
                            ,[Location1]
                            ,[NextProcess2]
                            ,[Location2]
                            ,[CustomerDeliveryDate]
                            ,[CustomerDeliveryTimeClass]
                            ,[CustomerDeliverySlipNumber]
                            ,[CustomerDeliverySlipRowNumber]
                            ,[CustomerCode]
                            ,[CustomerClass]
                            ,[CustomerName]
                            ,[CustomerProductCode]
                            ,[CustomerProductAbbreviation]
                            ,[CustomerProductManagementClass]
                            ,[CustomerProductLabelBranchNumber]
                            ,[CustomerNextProcess1]
                            ,[CustomerLocation1]
                            ,[CustomerNextProcess2]
                            ,[CustomerLocation2]
                            ,[CustomerOrderNumber]
                            ,[CustomerOrderClass]
                            ,[LotQuantity]
                            ,[FractionQuantity]
                            ,[Quantity]
                            ,[Packing]
                            ,[PackingCount]
                            ,[LotNumber]
                            ,[InvoiceNumber]
                            ,[ExpirationDate]
                            ,[DeleteFlag]
                            ,[DeleteShipmentID]
                            ,[Remark]
                        FROM [D_Shipment]
                        WHERE (1=1)
                            AND DepoID = @DepoID
                            AND ShipmentDate >= @ShipmentDateStart
                            AND ShipmentDate <= @ShipmentDateStop
                            AND DeleteFlag = @DeleteFlag
                        ";
            var param = new
            {
                DepoID = depoID,
                ShipmentDateStart = shipmentDateStart,
                ShipmentDateStop = shipmentDateStop,
                DeleteFlag = 0
            };
            registedData = (await connection.QueryAsync<D_ShipmentModel>(query, param, transaction)).ToList();
            return registedData;
        }


        [HttpPost("{companyID}")]
        public async Task<IActionResult> Post(int companyID, [FromBody] List<ScanPostBody> body)
        {
            var createDatetime = DateTime.Now;
            var registData = new RegistData();
            try
            {
                _logger.LogInformation("登録処理は開始");
                var stringJsonData = JsonConvert.SerializeObject(body);
                _logger.LogDebug(stringJsonData);

                // データベース名の取得
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) 
                {
                    _logger.LogError("会社IDが存在しないためデータベースの取得に失敗しました\"");
                    return Responce.ExNotFound("データベースの取得に失敗しました"); 
                }
                registData.DatabaseName = companys[0].DatabaseName;

                // 処理用のデータ詳細を取得

                var getRegistData = GetScanRegistData(body);

                if (getRegistData.result)
                {
                    registData.RegistDataRecords = getRegistData.registDatas;
                    registData.CreateDate = createDatetime;

                    (bool result, ReceivePostBackBody receivePostBackBody, string message) receivePostBackBody = (false, new ReceivePostBackBody(), "");
                    receivePostBackBody = await SaveChangeData(registData);

                    var endTime = DateTime.Now;
                    var elapsed = endTime - createDatetime;
                    var completeTime = elapsed.ToString(@"hh\:mm\:ss\.ffff");
                    _logger.LogInformation("登録処理は正常終了");
                    _logger.LogInformation("登録処理時間は: " + completeTime);

                    return Ok(receivePostBackBody.receivePostBackBody);
                }
                else
                {
                    _logger.LogError("Message   ：  スキャンデータの変換に失敗しました");
                    return Responce.ExBadRequest("スキャンデータの変換に失敗しました");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("登録処理は異常終了");
                _logger.LogError("Message   ：   " + ex.Message);
                return Responce.ExServerError(ex);
            }
        }


        private async Task<(bool result, ReceivePostBackBody receivePostBackBody, string message)> SaveChangeData(RegistData registData)
        {
            HandyReportLog handyReport = new HandyReportLog();
            ReceivePostBackBody receivePostBackBody = new ReceivePostBackBody();

            var connectionString = new GetConnectString(registData.DatabaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var listDataRegisted = new List<RegistDataRecord>();
                        var handyReportLog = new List<HandyReportLog>();
                        for (int j = 0; j < registData.RegistDataRecords.Count; j++)
                        {
                            var dtNow = DateTime.Now;
                            var postData = registData.RegistDataRecords[j].PostBody;
                            var qrCodeItem = registData.RegistDataRecords[j].QrcodeItem; // 製品かんばん
                            var qrCodeItem2 = registData.RegistDataRecords[j].QrcodeItem2; // 出荷かんばん

                            // スキャン履歴IDを取得
                            var scanRecordID = 0L;
                            var sql1 = @"
                                INSERT INTO D_ScanRecord
                                    (
                                         DepoID
                                        ,HandyUserID
                                        ,HandyOperationClass
                                        ,HandyOperationMessage
                                        ,Device
                                        ,HandyPageID
                                        ,StoreInFlag
                                        ,StoreOutFlag
                                        ,ScanStoreAddress1
                                        ,ScanStoreAddress2
                                        ,InputQuantity
                                        ,InputPackingCount
                                        ,ScanString1
                                        ,ScanString2
                                        ,ScanChangeData
                                        ,ScanTime
                                        ,Latitude
                                        ,Longitude
                                        ,CreateDate
                                    ) 
                                    OUTPUT 
                                       INSERTED.ScanRecordID
                                VALUES
                                    (
                                         @DepoID
                                        ,@HandyUserID
                                        ,@HandyOperationClass
                                        ,@HandyOperationMessage
                                        ,@Device
                                        ,@HandyPageID
                                        ,@StoreInFlag
                                        ,@StoreOutFlag
                                        ,@ScanStoreAddress1
                                        ,@ScanStoreAddress2
                                        ,@InputQuantity
                                        ,@InputPackingCount
                                        ,@ScanString1
                                        ,@ScanString2
                                        ,@ScanChangeData
                                        ,@ScanTime
                                        ,@Latitude
                                        ,@Longitude
                                        ,@CreateDate
                                    )
                                ";
                            var scanString1 = postData.ScanString1;
                            var scanString2 = postData.ScanString2;
                            if(postData.HandyOperationClass == 0 && !string.IsNullOrWhiteSpace(scanString1) && !string.IsNullOrWhiteSpace(scanString2))
                            {
                                var temString = scanString1;
                                scanString1 = scanString2;
                                scanString2 = temString;
                            }
                            var param1 = new
                            {
                                DepoID = postData.DepoID,
                                HandyUserID = postData.HandyUserID,
                                HandyOperationClass = postData.HandyOperationClass,
                                HandyOperationMessage = postData.HandyOperationMessage,
                                Device = postData.Device,
                                HandyPageID = postData.HandyPageID,
                                StoreInFlag = postData.StoreInFlag,
                                StoreOutFlag = true,
                                ScanStoreAddress1 = postData.ScanStoreAddress1,
                                ScanStoreAddress2 = postData.ScanStoreAddress2,
                                InputQuantity = postData.InputQuantity,
                                InputPackingCount = postData.InputPackingCount,
                                ScanString1 = scanString1,
                                ScanString2 = scanString2,
                                ScanChangeData = postData.ScanChangeData,
                                ScanTime = postData.ScanTime,
                                Latitude = postData.Latitude,
                                Longitude = postData.Longitude,
                                CreateDate = registData.CreateDate
                            };
                            try
                            {
                                scanRecordID = await connection.QuerySingleAsync<long>(sql1, param1, tran);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                                // スキップ
                                continue;
                            }

                            //// ハンディレポートログに格納
                            //if (!string.IsNullOrWhiteSpace(scanString1))
                            //    handyReportLog.Add(new HandyReportLog() { ScanRecordID = scanRecordID, HandyReport = JsonConvert.SerializeObject(qrCodeItem) });
                            //if (!string.IsNullOrWhiteSpace(scanString2))
                            //    handyReportLog.Add(new HandyReportLog() { ScanRecordID = scanRecordID, HandyReport = JsonConvert.SerializeObject(qrCodeItem2) });

                            // スキャンOK以外は、ここで終了する
                            // Error情報を記録するのみ
                            if (postData.HandyOperationClass != 0)
                            {
                                // スキップ
                                continue;
                            }

                            var registedDatas = await GetShippedDataByDeliveryDate(registData.DatabaseName, postData.DepoID, postData.ProcessDate, postData.ProcessDate, connection, tran);
                            var checkRegisted = registedDatas.Where(x =>
                                x.CustomerDeliveryDate == Convert.ToDateTime(qrCodeItem2.DeleveryDate)
                                && Convert.ToInt32(x.CustomerDeliveryTimeClass) == Convert.ToInt32(qrCodeItem2.DeliveryTimeClass)
                                && x.ProductCode == qrCodeItem2.ProductCode
                                    && x.LotQuantity == qrCodeItem2.Quantity
                                && x.CustomerProductLabelBranchNumber == qrCodeItem2.ProductLabelBranchNumber
                                ).FirstOrDefault();
                            if (checkRegisted != null)
                            {
                                // ハンディレポートログに格納
                                this.RegistedDataAdd(scanRecordID, listDataRegisted, handyReportLog, postData, qrCodeItem, qrCodeItem2);
                                receivePostBackBody.AlreadyRegisteredDatas.Add(qrCodeItem2); // 出荷かんばん
                                receivePostBackBody.AlreadyRegisteredDataCount++;
                                //receivePostBackBody.AlreadyRegisteredDatas.Add(qrCodeItem); // 製品かんばん
                                continue;
                            }

                            // 出庫実績をDBに登録
                            var storeOutID = 0L;
                            var sql2 = @"
                                INSERT INTO D_StoreOut
                                    (
                                         ScanRecordID
                                        ,ShipmentInstructionDetailID
                                        ,StoreInID
                                        ,DepoID
                                        ,StoreOutDate
                                        ,ProductCode
                                        ,Quantity
                                        ,Packing
                                        ,PackingCount
                                        ,StockLocation1
                                        ,StockLocation2
                                        ,AdjustmentFlag
                                        ,Remark
                                        ,DeleteFlag
                                        ,DeleteStoreOutID
                                        ,CreateDate
                                        ,CreateUserID
                                        ,UpdateDate
                                        ,UpdateUserID
                                    ) 
                                    OUTPUT
                                       INSERTED.StoreOutID
                                VALUES
                                    (
                                         @ScanRecordID
                                        ,@ShipmentInstructionDetailID
                                        ,@StoreInID
                                        ,@DepoID
                                        ,@StoreOutDate
                                        ,@ProductCode
                                        ,@Quantity
                                        ,@Packing
                                        ,@PackingCount
                                        ,@StockLocation1
                                        ,@StockLocation2
                                        ,@AdjustmentFlag
                                        ,@Remark
                                        ,@DeleteFlag
                                        ,@DeleteStoreOutID
                                        ,@CreateDate
                                        ,@CreateUserID
                                        ,@UpdateDate
                                        ,@UpdateUserID
                                    )
                                ";
                            storeOutID = await connection.QuerySingleAsync<long>(sql2, new
                            {
                                ScanRecordID = scanRecordID,
                                ShipmentInstructionDetailID = defaultBigInt,
                                StoreInID = defaultBigInt,
                                DepoID = postData.DepoID,
                                StoreOutDate = postData.ProcessDate,
                                ProductCode = qrCodeItem.ProductCode,
                                Quantity = qrCodeItem.Quantity,
                                Packing = qrCodeItem.Packing,
                                PackingCount = 1,
                                StockLocation1 = qrCodeItem.NextProcess1,
                                StockLocation2 = qrCodeItem.Location1,
                                AdjustmentFlag = defaultBool,
                                Remark = defaultString,
                                DeleteFlag = defaultBool,
                                DeleteStoreOutID = defaultBool,
                                CreateDate = dtNow,
                                CreateUserID = postData.HandyUserID,
                                UpdateDate = dtNow,
                                UpdateUserID = postData.HandyUserID
                            }, tran);

                            var shipmentID = 0L;
                            var sql3 = @"
                                INSERT INTO D_Shipment
                                    (
                                         ScanRecordID
                                        ,ShipmentInstructionDetailID
                                        ,StoreOutID
                                        ,HandyMatchClass
                                        ,HandyMatchResult
                                        ,ShipmentDate
                                        ,DepoID
                                        ,DeliveryDate
                                        ,DeliveryTimeClass
                                        ,DeliverySlipNumber
                                        ,DeliverySlipRowNumber
                                        ,SupplierCode
                                        ,SupplierClass
                                        ,ProductCode
                                        ,ProductAbbreviation
                                        ,ProductManagementClass
                                        ,ProductLabelBranchNumber
                                        ,NextProcess1
                                        ,Location1
                                        ,NextProcess2
                                        ,Location2
                                        ,CustomerDeliveryDate
                                        ,CustomerDeliveryTimeClass
                                        ,CustomerDeliverySlipNumber
                                        ,CustomerDeliverySlipRowNumber
                                        ,CustomerCode
                                        ,CustomerClass
                                        ,CustomerName
                                        ,CustomerProductCode
                                        ,CustomerProductAbbreviation
                                        ,CustomerProductManagementClass
                                        ,CustomerProductLabelBranchNumber
                                        ,CustomerNextProcess1
                                        ,CustomerLocation1
                                        ,CustomerNextProcess2
                                        ,CustomerLocation2
                                        ,CustomerOrderNumber
                                        ,CustomerOrderClass
                                        ,LotQuantity
                                        ,FractionQuantity
                                        ,Quantity
                                        ,Packing
                                        ,PackingCount
                                        ,LotNumber
                                        ,InvoiceNumber
                                        ,ExpirationDate
                                        ,DeleteFlag
                                        ,DeleteShipmentID
                                        ,Remark
                                        ,CreateDate
                                        ,CreateHandyUserID
                                        ,CreateUserID
                                        ,UpdateDate
                                        ,UpdateUserID

                                    ) 
                                    OUTPUT 
                                       INSERTED.ShipmentID
                                VALUES
                                    (
                                         @ScanRecordID
                                        ,@ShipmentInstructionDetailID
                                        ,@StoreOutID
                                        ,@HandyMatchClass
                                        ,@HandyMatchResult
                                        ,@ShipmentDate
                                        ,@DepoID
                                        ,@DeliveryDate
                                        ,@DeliveryTimeClass
                                        ,@DeliverySlipNumber
                                        ,@DeliverySlipRowNumber
                                        ,@SupplierCode
                                        ,@SupplierClass
                                        ,@ProductCode
                                        ,@ProductAbbreviation
                                        ,@ProductManagementClass
                                        ,@ProductLabelBranchNumber
                                        ,@NextProcess1
                                        ,@Location1
                                        ,@NextProcess2
                                        ,@Location2
                                        ,@CustomerDeliveryDate
                                        ,@CustomerDeliveryTimeClass
                                        ,@CustomerDeliverySlipNumber
                                        ,@CustomerDeliverySlipRowNumber
                                        ,@CustomerCode
                                        ,@CustomerClass
                                        ,@CustomerName
                                        ,@CustomerProductCode
                                        ,@CustomerProductAbbreviation
                                        ,@CustomerProductManagementClass
                                        ,@CustomerProductLabelBranchNumber
                                        ,@CustomerNextProcess1
                                        ,@CustomerLocation1
                                        ,@CustomerNextProcess2
                                        ,@CustomerLocation2
                                        ,@CustomerOrderNumber
                                        ,@CustomerOrderClass
                                        ,@LotQuantity
                                        ,@FractionQuantity
                                        ,@Quantity
                                        ,@Packing
                                        ,@PackingCount
                                        ,@LotNumber
                                        ,@InvoiceNumber
                                        ,@ExpirationDate
                                        ,@DeleteFlag
                                        ,@DeleteShipmentID
                                        ,@Remark
                                        ,@CreateDate
                                        ,@CreateHandyUserID
                                        ,@CreateUserID
                                        ,@UpdateDate
                                        ,@UpdateUserID
                                    )
                                ";

                            shipmentID = await connection.QuerySingleAsync<long>(sql3, new
                            {
                                ScanRecordID = scanRecordID,
                                ShipmentInstructionDetailID = defaultBigInt,
                                StoreOutID = storeOutID,
                                HandyMatchClass = defaultString,
                                HandyMatchResult = defaultString,
                                ShipmentDate = postData.ProcessDate,
                                DepoID = postData.DepoID,
                                DeliveryDate = defaultDateTime,
                                DeliveryTimeClass = defaultString,
                                DeliverySlipNumber = defaultString,
                                DeliverySlipRowNumber = defaultInt,
                                SupplierCode = qrCodeItem.SupplierCode,
                                SupplierClass = defaultString,
                                ProductCode = qrCodeItem.ProductCode,
                                ProductAbbreviation = qrCodeItem.ProductAbbreviation,
                                ProductManagementClass = defaultString,
                                ProductLabelBranchNumber = qrCodeItem.ProductLabelBranchNumber,
                                NextProcess1 = qrCodeItem.NextProcess1,
                                Location1 = qrCodeItem.Location1,
                                NextProcess2 = defaultString,
                                Location2 = defaultString,
                                CustomerDeliveryDate = qrCodeItem2.DeleveryDate,
                                CustomerDeliveryTimeClass = qrCodeItem2.DeliveryTimeClass,
                                CustomerDeliverySlipNumber = defaultString,
                                CustomerDeliverySlipRowNumber = defaultInt,
                                CustomerCode = defaultString,
                                CustomerClass = defaultString,
                                CustomerName = defaultString,
                                CustomerProductCode = defaultString,
                                CustomerProductAbbreviation = defaultString,
                                CustomerProductManagementClass = defaultString,
                                CustomerProductLabelBranchNumber = qrCodeItem2.ProductLabelBranchNumber,
                                CustomerNextProcess1 = defaultString,
                                CustomerLocation1 = defaultString,
                                CustomerNextProcess2 = defaultString,
                                CustomerLocation2 = defaultString,
                                CustomerOrderNumber = defaultString,
                                CustomerOrderClass = defaultString,
                                LotQuantity = qrCodeItem.Quantity,
                                FractionQuantity = defaultInt,
                                Quantity = qrCodeItem.Quantity,
                                Packing = qrCodeItem.Packing,
                                PackingCount = 1,
                                LotNumber = defaultString,
                                InvoiceNumber = defaultString,
                                ExpirationDate = defaultDateTime,
                                DeleteFlag = defaultBool,
                                DeleteShipmentID = defaultBool,
                                Remark = defaultString,
                                CreateDate = dtNow,
                                CreateHandyUserID = postData.HandyUserID,
                                CreateUserID = defaultInt,
                                UpdateDate = dtNow,
                                UpdateUserID = postData.HandyUserID
                            }, tran);

                            receivePostBackBody.SuccessDataCount++;
                        }

                        // ハンディレポートログをDBに登録
                        foreach (var item in handyReportLog)
                        {
                            var sql_reporthandy_log = @$"
                                                INSERT INTO D_HandyReportLog
                                                (
                                                       ScanRecordID
                                                      ,HandyReport
                                                )
                                                VALUES
                                                (
                                                       @ScanRecordID
                                                      ,@HandyReport
                                                )
                                            ";
                            var result = await connection.ExecuteAsync(sql_reporthandy_log, new
                            {
                                ScanRecordID = item.ScanRecordID,
                                HandyReport = item.HandyReport
                            }, tran);
                        }

                        tran.Commit();
                        return (true, receivePostBackBody, "出荷実績データの登録が完了しました");
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        receivePostBackBody.SuccessDataCount = 0;
                        throw;
                    }
                }
            }
        }


        private void RegistedDataAdd(long scanRecordID, List<RegistDataRecord> registDataRecords, List<HandyReportLog> handyReportLogs, ScanPostBody scanPost, QrcodeItem qrcodeItem, QrcodeItem qrcodeItem2)
        {
            // レスポンス 既に登録済データに格納
            registDataRecords.Add(new RegistDataRecord() { PostBody = scanPost, QrcodeItem = qrcodeItem });

            // ハンディレポートログに格納
            handyReportLogs.Add(new HandyReportLog() { ScanRecordID = scanRecordID, HandyReport = JsonConvert.SerializeObject(qrcodeItem2) });
            //handyReportLogs.Add(new HandyReportLog() { ScanRecordID = scanRecordID, HandyReport = JsonConvert.SerializeObject(qrcodeItem) });
        }
    }
}
