using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using static SakaguraAGFWebApi.Models.HandyReportLogModel;
using static SakaguraAGFWebApi.Models.ReceiveModel;
using static SakaguraAGFWebApi.Models.ScanCommonModel;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReceiveController : ControllerBase
    {
        [HttpGet("{companyID}")]
        public IActionResult Get(int companyID, int depoID, string receiveDateStart, string receiveDateEnd)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExBadRequest("会社情報の取得に失敗しました");
            var databaseName = companys[0].DatabaseName;
            if (String.IsNullOrEmpty(databaseName)) return Responce.ExBadRequest("データベースの取得に失敗しました");

            List<D_Receive> receives = new List<D_Receive>();

            try
            {
                receives = ReceiveModel.GetReceiveByReceiveDate(databaseName, depoID, receiveDateStart, receiveDateEnd);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            return receives.Count() == 0 ? NotFound("データが存在しません") : Ok(receives);

        }

        // POST: api/<controller>
        [HttpPost("{companyID}")]
        public IActionResult Post(int companyID, [FromBody]List<ScanPostBody> body)
        {
            var createDatetime = DateTime.Now;
            var registData = new RegistData();

            // データベース名の取得
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            registData.DatabaseName = companys[0].DatabaseName;

            // 処理用のデータ詳細を取得

            var getRegistData = GetScanRegistData(body);
            if (getRegistData.result)
            {
                registData.RegistDataRecords = getRegistData.registDatas;
                registData.CreateDate = createDatetime;

                (bool result, ReceivePostBackBody receivePostBackBody, string message) receivePostBackBody = (false, new ReceivePostBackBody(), "");

                try
                {
                    receivePostBackBody = Regist(registData);
                }
                catch (Exception ex)
                {
                    return Responce.ExServerError(ex);
                }

                if (receivePostBackBody.result)
                {
                    return Ok(receivePostBackBody.receivePostBackBody);
                }
                else
                {
                    return Responce.ExBadRequest(receivePostBackBody.message);
                }

            }
            else
            {
                return Responce.ExBadRequest("スキャンデータの変換に失敗しました");
            }

        }

        private (bool result, ReceivePostBackBody receivePostBackBody, string message) Regist(RegistData registData)
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

                        for (int j = 0; j <= registData.RegistDataRecords.Count - 1; j++)
                        {
                            HandyReportLogModel.HandyReportLog handyReportLog = new HandyReportLogModel.HandyReportLog();
                            QrcodeModel.QrcodeItem alreadyRegisteredData = new QrcodeModel.QrcodeItem();

                            var post = registData.RegistDataRecords[j].PostBody;
                            var qrcode = registData.RegistDataRecords[j].QrcodeItem;

                            #region スキャン履歴をINSERT

                            // スキャン履歴IDを取得
                            long scanRecordID = 0;
                            string sql1 = @"
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

                            var param1 = new
                            {
                                post.DepoID,
                                post.HandyUserID,
                                post.HandyOperationClass,
                                post.HandyOperationMessage,
                                post.Device,
                                post.HandyPageID,
                                post.StoreInFlag,
                                StoreOutFlag = false,
                                post.ScanStoreAddress1,
                                post.ScanStoreAddress2,
                                post.InputQuantity,
                                post.InputPackingCount,
                                post.ScanString1,
                                post.ScanString2,
                                post.ScanChangeData,
                                post.ScanTime,
                                post.Latitude,
                                post.Longitude,
                                registData.CreateDate
                            };
                            var result1 = connection.QuerySingle<long>(sql1, param1, tran);

                            if (result1 < 1)
                            {
                                tran.Rollback();
                                return (false, receivePostBackBody, "スキャン実績の登録に失敗しました");
                            }
                            else
                            {
                                scanRecordID = result1;
                            }

                            #endregion

                            // スキャンOK以外は、ここで終了する
                            // Error情報を記録するのみ
                            if (post.HandyOperationClass != 0)
                            {
                                // スキップ
                                continue;
                            }

                            #region 入荷済みチェック

                            string sql2 = @"
                                    SELECT
                                        COUNT(*) AS DataCount
                                    FROM D_Receive AS A 
                                    WHERE
                                        (
                                            A.ReceiveDate <= @ReceiveDate
                                            AND
                                            A.ReceiveDate >= @DuplicateCheckStartReceiveDate
                                        )
                                        AND DeleteFlag = @DeleteFlag
                                        AND A.DepoID = @DepoID
                                        AND A.SupplierCode = @SupplierCode
                                        AND A.SupplierClass = @SupplierClass
                                        AND A.ProductCode = @ProductCode
                                        AND A.ProductLabelBranchNumber = @ProductLabelBranchNumber
                                        AND A.Quantity = @Quantity
                                        AND A.NextProcess1 = @NextProcess1
                                        AND A.NextProcess2 = @NextProcess2
                                        AND A.Location1 = @Location1
                                        AND A.Location2 = @Location2
                                        AND A.Packing = @Packing
                                     ";

                            var param2 = new
                            {
                                ReceiveDate = post.ProcessDate,
                                DuplicateCheckStartReceiveDate = post.DuplicateCheckStartProcessDate,
                                post.DepoID,
                                DeleteFlag = 0,
                                qrcode.SupplierCode,
                                qrcode.SupplierClass,
                                qrcode.ProductCode,
                                qrcode.ProductLabelBranchNumber,
                                qrcode.NextProcess1,
                                qrcode.NextProcess2,
                                qrcode.Location1,
                                qrcode.Location2,
                                qrcode.Quantity,
                                qrcode.Packing
                            };

                            var result2 = connection.QueryFirstOrDefault<int>(sql2, param2, tran);

                            if (result2 > 0)
                            {
                                // 既に入荷登録がある場合
                                alreadyRegisteredData = qrcode;

                                receivePostBackBody.AlreadyRegisteredDatas.Add(alreadyRegisteredData);
                                receivePostBackBody.AlreadyRegisteredDataCount++;

                                //string reportJson = JsonConvert.SerializeObject(alreadyRegisteredData);
                                //var handyReportInsertResult = Util.InsertHandyReportLog(registData.DatabaseName, scanRecordID, reportJson);
                                handyReportLog.ScanRecordID = scanRecordID;
                                handyReportLog.HandyReport = JsonConvert.SerializeObject(alreadyRegisteredData);
                                handyReport.HandyReportLogs.Add(handyReportLog);

                                // スキップ
                                continue;
                            }

                            #endregion

                            #region 入荷予定データの存在チェック

                            // 入荷予定詳細データのID取得
                            long receiveScheduleDetailID = 0;

                            // ↓2023/12/21 EDIかんばんで入荷した際は、入荷予定データを消しこむ処理を行いたいが、
                            // 工数が取れないので一旦コメントアウト(山本)

                            //// スキャンデータ（QR）に「納期」と「納品書番号」がある場合のみ
                            //if (!String.IsNullOrEmpty(qrcode.DeliveryDate) && !String.IsNullOrEmpty(qrcode.DeliverySlipNumber))
                            //{
                            //    string sql3 = @"
                            //                    SELECT
                            //                        A.ReceiveScheduleDetailID
                            //                    FROM D_ReceiveScheduleDetail AS A
                            //                    LEFT OUTER JOIN D_ReceiveScheduleHeader AS B
                            //                        ON   A.ReceiveScheduleDate = B.ReceiveScheduleDate
                            //                        AND A.DeliverySlipNumber = B.DeliverySlipNumber
                            //                    WHERE (1=1)
                            //                        AND A.ReceiveScheduleDate = @ReceiveScheduleDate
                            //                        AND A.DeliverySlipNumber = @DeliverySlipNumber
                            //                        AND B.ReceiveTimeClass = @ReceiveTimeClass
                            //                        AND A.ProductCode = @ProductCode
                            //                        AND A.LotQuantity = @LotQuantity
                            //                        AND A.CancelFlag = @CancelFlag
                            //                        ";

                            //    var param3 = new
                            //    {
                            //        ReceiveScheduleDate = qrcode.DeliveryDate,
                            //        qrcode.DeliverySlipNumber,
                            //        ReceiveTimeClass = qrcode.DeliveryTimeClass,
                            //        qrcode.ProductCode,
                            //        LotQuantity = qrcode.Quantity,
                            //        CancelFlag = 0
                            //    };

                            //    var result3 = connection.Query<long>(sql3, param3, tran).ToList();

                            //    if (result3 == null || result3.Count == 0)
                            //    {
                            //        // データの取得に失敗した場合
                            //        tran.Rollback();
                            //        return (false, receivePostBackBody, "入荷予定データの取得に失敗しました");
                            //    }
                            //    else if (result3[0] > 1)
                            //    {
                            //        // 入荷予定が複数存在した場合
                            //        tran.Rollback();
                            //        return (false, receivePostBackBody, "入荷予定データの取得に失敗しました");
                            //    }
                            //    else if (result3[0] == 0)
                            //    {
                            //        // 入荷予定が存在しない場合
                            //    }
                            //    else if (result3[0] == 1)
                            //    {
                            //        // OK
                            //        receiveScheduleDetailID = result3[0];
                            //    }
                            //    else
                            //    {
                            //        // その他
                            //    }

                            //}

                            #endregion

                            #region 入荷（入庫）合計数量の計算

                            // 箱数セット
                            var packingCount = (post.InputPackingCount == 0) ? 1 : post.InputPackingCount;
                            // 入荷合計数量の算出
                            var totalQuantity = Util.GetRegistTotalQuantity(post.InputQuantity, post.InputPackingCount, qrcode.Quantity);

                            if (totalQuantity == 0)
                            {
                                tran.Rollback();
                                return (false, receivePostBackBody, "入庫合計数量が0のデータは登録できません");
                            }

                            #endregion

                            #region 入荷実績をINSERT

                            // 入荷実績IDを取得
                            int receiveID = 0;
                            string sql4 = @"
                                                INSERT INTO D_Receive 
                                                (
                                                   ScanRecordID
                                                  ,ReceiveScheduleDetailID
                                                  ,ReceiveDate
                                                  ,DepoID
                                                  ,DeliveryDate
                                                  ,DeliveryTimeClass
                                                  ,DeliverySlipNumber
                                                  ,DeliverySlipRowNumber
                                                  ,SupplierCode
                                                  ,SupplierClass
                                                  ,CustomerCode
                                                  ,CustomerClass
                                                  ,ProductCode
                                                  ,ProductAbbreviation
                                                  ,ProductManagementClass
                                                  ,ProductLabelBranchNumber
                                                  ,CustomerProductCode
                                                  ,CustomerProductAbbreviation
                                                  ,CustomerProductLabelBranchNumber
                                                  ,NextProcess1
                                                  ,Location1
                                                  ,NextProcess2
                                                  ,Location2
                                                  ,LotQuantity
                                                  ,FractionQuantity
                                                  ,DefectiveQuantity
                                                  ,Quantity
                                                  ,Packing
                                                  ,PackingCount
                                                  ,LotNumber
                                                  ,InvoiceNumber
                                                  ,OrderNumber
                                                  ,ExpirationDate
                                                  ,CostPrice
                                                  ,DeleteFlag
                                                  ,DeleteReceiveID
                                                  ,Remark
                                                  ,CreateDate
                                                  ,CreateUserID
                                                  ,UpdateDate
                                                  ,UpdateUserID
                                                )
                                                OUTPUT 
                                                   INSERTED.ReceiveID
                                                VALUES
                                                (
                                                   @ScanRecordID
                                                  ,@ReceiveScheduleDetailID
                                                  ,@ReceiveDate
                                                  ,@DepoID
                                                  ,@DeliveryDate
                                                  ,@DeliveryTimeClass
                                                  ,@DeliverySlipNumber
                                                  ,@DeliverySlipRowNumber
                                                  ,@SupplierCode
                                                  ,@SupplierClass
                                                  ,@CustomerCode
                                                  ,@CustomerClass
                                                  ,@ProductCode
                                                  ,@ProductAbbreviation
                                                  ,@ProductManagementClass
                                                  ,@ProductLabelBranchNumber
                                                  ,@CustomerProductCode
                                                  ,@CustomerProductAbbreviation
                                                  ,@CustomerProductLabelBranchNumber
                                                  ,@NextProcess1
                                                  ,@Location1
                                                  ,@NextProcess2
                                                  ,@Location2
                                                  ,@LotQuantity
                                                  ,@FractionQuantity
                                                  ,@DefectiveQuantity
                                                  ,@Quantity
                                                  ,@Packing
                                                  ,@PackingCount
                                                  ,@LotNumber
                                                  ,@InvoiceNumber
                                                  ,@OrderNumber
                                                  ,@ExpirationDate
                                                  ,@CostPrice
                                                  ,@DeleteFlag
                                                  ,@DeleteReceiveID
                                                  ,@Remark
                                                  ,@CreateDate
                                                  ,@CreateUserID
                                                  ,@UpdateDate
                                                  ,@UpdateUserID
                                                )
                                        ";

                            var param4 = new
                            {
                                ScanRecordID = scanRecordID,
                                ReceiveScheduleDetailID = receiveScheduleDetailID,
                                post.DepoID,
                                qrcode.DeliveryDate,
                                qrcode.DeliveryTimeClass,
                                qrcode.DeliverySlipNumber,
                                DeliverySlipRowNumber= 0,
                                qrcode.SupplierCode,
                                qrcode.SupplierClass,
                                qrcode.CustomerCode,
                                qrcode.CustomerClass,
                                ReceiveDate = post.ProcessDate,
                                qrcode.ProductCode,
                                qrcode.ProductAbbreviation,
                                qrcode.ProductLabelBranchNumber,
                                qrcode.CustomerProductCode,
                                qrcode.CustomerProductAbbreviation,
                                qrcode.CustomerProductLabelBranchNumber,
                                ProductManagementClass = "",
                                qrcode.NextProcess1,
                                qrcode.Location1,
                                qrcode.NextProcess2,
                                Location2 ="",
                                LotQuantity = qrcode.Quantity,
                                FractionQuantity = 0,
                                DefectiveQuantity = 0,
                                Quantity = totalQuantity,
                                qrcode.Packing,
                                PackingCount = packingCount,
                                LotNumber = "",
                                InvoiceNumber = "",
                                OrderNumber = "",
                                ExpirationDate = "",
                                CostPrice = 0,
                                DeleteFlag = 0,
                                DeleteReceiveID = "",
                                Remark = "",
                                registData.CreateDate,
                                CreateUserID = post.HandyUserID,
                                UpdateDate = registData.CreateDate,
                                UpdateUserID = post.HandyUserID
                            };

                            var result4 = connection.QuerySingle<int>(sql4, param4, tran);
                            if (result4 < 1)
                            {
                                tran.Rollback();
                                return (false, receivePostBackBody, "入荷実績データの登録に失敗しました");
                            }
                            else
                            {
                                receiveID = result4;
                                //receiveSuccessDataCount++;
                                receivePostBackBody.SuccessDataCount++;
                            }
                            
                            #endregion

                            #region 在庫入庫データをINSERT

                            // 在庫入庫フラグが1の場合のみ
                            if (post.StoreInFlag)
                            {
                                string sql5 = @"
                                                INSERT INTO D_StoreIn 
                                                (
                                                       ReceiveID
                                                      ,ScanRecordID
                                                      ,DepoID
                                                      ,StoreInDate
                                                      ,ProductCode
                                                      ,Quantity
                                                      ,Packing
                                                      ,PackingCount
                                                      ,StockLocation1
                                                      ,StockLocation2
                                                      ,Remark
                                                      ,DeleteFlag
                                                      ,DeleteStoreInID
                                                      ,CreateDate
                                                      ,CreateUserID
                                                      ,UpdateDate
                                                      ,UpdateUserID
                                                )
                                                VALUES
                                                (
                                                       @ReceiveID
                                                      ,@ScanRecordID
                                                      ,@DepoID
                                                      ,@StoreInDate
                                                      ,@ProductCode
                                                      ,@Quantity
                                                      ,@Packing
                                                      ,@PackingCount
                                                      ,@StockLocation1
                                                      ,@StockLocation2
                                                      ,@Remark
                                                      ,@DeleteFlag
                                                      ,@DeleteStoreInID
                                                      ,@CreateDate
                                                      ,@CreateUserID
                                                      ,@UpdateDate
                                                      ,@UpdateUserID
                                                )
                                                    ";

                                var param5 = new
                                {
                                    ReceiveID = receiveID,
                                    ScanRecordID = scanRecordID,
                                    post.DepoID,
                                    StoreInDate = post.ProcessDate,
                                    qrcode.ProductCode,
                                    qrcode.Quantity,
                                    qrcode.Packing,
                                    PackingCount = 1, // 入庫データ作成は１箱ずつ
                                    StockLocation1 = (post.ScanStoreAddress1 == "") ? qrcode.NextProcess1 : post.ScanStoreAddress1,
                                    StockLocation2 = (post.ScanStoreAddress2 == "") ? qrcode.Location1 : post.ScanStoreAddress2,
                                    Remark = "",
                                    DeleteFlag = 0,
                                    DeleteStoreInID = 0,
                                    registData.CreateDate,
                                    CreateUserID = post.HandyUserID,
                                    UpdateDate = registData.CreateDate,
                                    UpdateUserID = post.HandyUserID
                                };

                                // 在庫単位ごとに入庫データを作成（入庫単位＝出庫単位）
                                // 箱数の分だけINSERTを繰り返す
                                for (int x = 0; x < packingCount; ++x)
                                {
                                    // 処理件数を返す
                                    var result5 = connection.Execute(sql5, param5, tran);

                                    if (result5 < 1)
                                    {
                                        tran.Rollback();
                                        return (false, receivePostBackBody, "在庫入庫データの登録に失敗しました");
                                    }
                                    else
                                    {
                                        // スキップ
                                    }
                                }

                                #endregion

                            }

                        }

                        if (handyReport.HandyReportLogs.Count > 0)
                        {
                            var handyReportInsertResult =  HandyReportLogModel.InsertHandyReportLog(registData.DatabaseName, handyReport.HandyReportLogs);
                        }

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, receivePostBackBody, "データの登録に失敗しました");
                    }

                    tran.Commit();
                    return (true, receivePostBackBody, "");
                }
            }

        }


    }
}
