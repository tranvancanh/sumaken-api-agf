using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using static SakaguraAGFWebApi.Models.HandyReportLogModel;
using static SakaguraAGFWebApi.Models.ScanCommonModel;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReturnStoreAddressController : ControllerBase
    {
        // POST: api/<controller>
        [HttpPost("{companyID}")]
        public IActionResult Post(int companyID, [FromBody]List<ScanPostBody> body)
        {
            var createDatetime = DateTime.Now;

            var registData = new ReceiveModel.RegistData();

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

                (bool result, ReturnStoreAddresseModel.ReturnStoreAddressePostBackBody postBackBody, string message) returnStoreAddressPostBackBody = (false, new ReturnStoreAddresseModel.ReturnStoreAddressePostBackBody(), "");

                try
                {
                    returnStoreAddressPostBackBody = Regist(registData);
                }
                catch (Exception ex)
                {
                    return Responce.ExServerError(ex);
                }

                if (returnStoreAddressPostBackBody.result)
                {
                    return Ok(returnStoreAddressPostBackBody.postBackBody);
                }
                else
                {
                    return Responce.ExBadRequest(returnStoreAddressPostBackBody.message);
                }

            }
            else
            {
                return Responce.ExBadRequest("スキャンデータの変換に失敗しました");
            }

        }

        private (bool result, ReturnStoreAddresseModel.ReturnStoreAddressePostBackBody returnStoreAddressPostBackBody, string message) Regist(ReceiveModel.RegistData registData)
        {

            HandyReportLog handyReport  = new HandyReportLog();
            ReturnStoreAddresseModel.ReturnStoreAddressePostBackBody returnStoreAddressPostBack = new ReturnStoreAddresseModel.ReturnStoreAddressePostBackBody();

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
                            QrcodeModel.QrcodeItem notFoundData = new QrcodeModel.QrcodeItem();

                            var post = registData.RegistDataRecords[j].PostBody;
                            var qrcode = registData.RegistDataRecords[j].QrcodeItem;

                            #region スキャン履歴をINSERT

                            // スキャン実績を登録→IDを返す
                            long scanRecordID = 0;
                            string sql1 = $@"
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
                                return (false, returnStoreAddressPostBack, "スキャン実績の登録に失敗しました");
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

                            #region 移動元入庫データの取得

                            StoreInModel.D_StoreIn targetStoreInData = new StoreInModel.D_StoreIn();

                            string sql2AddWhere = "";

                            string sql2 = $@"
                                SELECT
                                     A.StoreInID
                                    ,A.ScanRecordID
                                    ,A.ReceiveID
                                    ,A.DepoID
                                    ,A.StoreInDate
                                    ,A.ProductCode
                                    ,A.Quantity
                                    ,A.Packing
                                    ,A.PackingCount
                                    ,A.StockLocation1
                                    ,A.StockLocation2
                                    ,A.Remark
                                    ,A.CreateDate
                                    ,A.CreateUserID
                                FROM D_StoreIn AS A
                                LEFT OUTER JOIN D_StoreOut AS B ON A.StoreInID = B.StoreInID
                                WHERE (1=1)
                                AND A.StoreInID = 
                                    (
                                        SELECT
                                            TOP(1) SelectReceiveData.StoreInID
                                        FROM
                                        (
                                            SELECT
                                                 B.StoreInID
                                                ,B.StoreInDate
                                                ,B.CreateDate
                                            FROM D_Receive AS A
                                            LEFT OUTER JOIN D_StoreIn AS B ON A.ReceiveID = B.ReceiveID
                                            WHERE (1=1)
                                                AND A.SupplierCode = @SupplierCode
                                                AND A.ProductCode = @ProductCode
                                                AND A.ProductLabelBranchNumber = @ProductLabelBranchNumber
                                                AND A.Quantity = @Quantity
                                                AND A.Packing = @Packing
                                                AND A.NextProcess1 = @NextProcess1
                                                AND A.Location1 = @Location1
                                                AND A.NextProcess2 = @NextProcess2
                                                AND A.Location2 = @Location2
                                                AND B.DepoID = @DepoID --移動対象デポに入庫済み
                                                AND B.DeleteFlag = @DeleteFlag --移動元データが削除データではない
                                                AND B.StoreInDate <= @StoreInDate --移動元日付が、今回の入庫日以前
                                                AND EXISTS ( --移動元データのロケが仮番地で存在している
                                                    SELECT *
                                                    FROM M_TemporaryStoreAddress AS C
                                                    WHERE (1=1)
                                                        AND C.DepoID = @DepoID
                                                        AND B.StockLocation1 = C.TemporaryStoreAddress1
                                                        AND B.StockLocation2 = C.TemporaryStoreAddress2
                                                 )"
                                                + sql2AddWhere 
                                                + $@" ) AS SelectReceiveData
                                            ORDER BY StoreInDate DESC, CreateDate DESC
                                   )
                                AND B.StoreOutID IS NULL --未出庫データ;";

                            // ワンウェイかんばん（納期・便や伝票番号などの記載があるかんばん）の場合
                            if (!String.IsNullOrEmpty(qrcode.DeliveryDate))
                            {
                                sql2AddWhere += $@"AND A.DeliveryDate = @DeliveryDate ";
                            }
                            else if (!String.IsNullOrEmpty(qrcode.DeliveryTimeClass))
                            {
                                sql2AddWhere += $@"AND A.DeliveryTimeClass = @DeliveryTimeClass ";
                            }
                            else if (!String.IsNullOrEmpty(qrcode.DeliverySlipNumber))
                            {
                                sql2AddWhere += $@"AND A.DeliverySlipNumber = @DeliverySlipNumber ";
                            }

                            var param2 = new
                            {
                                qrcode.SupplierCode,
                                qrcode.ProductCode,
                                qrcode.ProductLabelBranchNumber,
                                qrcode.Quantity,
                                qrcode.Packing,
                                qrcode.NextProcess1,
                                qrcode.Location1,
                                qrcode.NextProcess2,
                                qrcode.Location2,
                                post.DepoID,
                                DeleteFlag = 0,
                                StoreInDate = post.ProcessDate,
                                qrcode.DeliveryDate,
                                qrcode.DeliveryTimeClass,
                                qrcode.DeliverySlipNumber
                            };

                            targetStoreInData = connection.QueryFirstOrDefault<StoreInModel.D_StoreIn>(sql2, param2, tran);

                            // 移動元の対象データが存在しない場合
                            if (targetStoreInData == null || targetStoreInData.StoreInID == 0)
                            {
                                notFoundData = qrcode;

                                // 移動失敗したデータを格納
                                returnStoreAddressPostBack.StoreInNotFoundDatas.Add(notFoundData);
                                returnStoreAddressPostBack.StoreInNotFoundDataCount++;

                                //string reportJson = JsonConvert.SerializeObject(notFoundData);
                                //var handyReportInsertResult =  Util.InsertHandyReportLog(registData.DatabaseName, scanRecordID, reportJson);
                                handyReportLog.ScanRecordID = scanRecordID;
                                handyReportLog.HandyReport = JsonConvert.SerializeObject(notFoundData);
                                handyReport.HandyReportLogs.Add(handyReportLog);

                                // スキップして次のデータ処理へ
                                continue;
                            }

                            // 移動元入庫データとスキャンしたデポが異なる場合
                            if (targetStoreInData.DepoID != post.DepoID)
                            {
                                tran.Rollback();
                                return (false, returnStoreAddressPostBack, "異なる倉庫への移動はできません");
                            }

                            #endregion

                            #region 在庫入庫データの保管場所を更新する


                            // 在庫入庫データを削除データに更新する
                            var sql3 = $@"
                                        UPDATE D_StoreIn
                                            SET
                                                 DeleteFlag = @DeleteFlag
                                                ,UpdateDate = @UpdateDate
                                                ,UpdateUserID = @UpdateUserID
                                            WHERE 1 = 1
                                                AND StoreInID = @StoreInID
                                        ;";

                            var param3 = new
                            {
                                targetStoreInData.StoreInID,
                                DeleteFlag = true,
                                UpdateDate = registData.CreateDate,
                                UpdateUserID = post.HandyUserID
                            };
                            var updateStoreInResult = connection.Execute(sql3, param3, tran);

                            if (updateStoreInResult != 1)
                            {
                                tran.Rollback();
                                return (false, returnStoreAddressPostBack, "移動データの更新に失敗しました");
                            }

                            var sql4 = $@"
                                            INSERT INTO D_StoreIn (
                                                   ScanRecordID
                                                  ,ReceiveID
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
                                            OUTPUT 
                                                INSERTED.StoreInID
                                            VALUES (
                                                   @ScanRecordID
                                                  ,@ReceiveID
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
                                        );";

                            var param4 = new
                            {
                                scanRecordID,
                                targetStoreInData.ReceiveID,
                                post.DepoID,
                                StoreInDate = post.ProcessDate,
                                targetStoreInData.ProductCode,
                                targetStoreInData.Quantity,
                                targetStoreInData.Packing,
                                targetStoreInData.PackingCount,
                                StockLocation1 = (post.ScanStoreAddress1 == "") ? qrcode.NextProcess1 : post.ScanStoreAddress1,
                                StockLocation2 = (post.ScanStoreAddress2 == "") ? qrcode.Location1 : post.ScanStoreAddress2,
                                targetStoreInData.Remark,
                                DeleteFlag = false,
                                DeleteStoreInID = targetStoreInData.StoreInID,
                                registData.CreateDate,
                                CreateUserID = post.HandyUserID,
                                UpdateDate = registData.CreateDate,
                                UpdateUserID = post.HandyUserID
                            };
                            var insertStoreInResult = connection.Execute(sql4, param4, tran);

                            if (insertStoreInResult != 1)
                            {
                                tran.Rollback();
                                return (false, returnStoreAddressPostBack, "移動データの更新に失敗しました");
                            }

                            #endregion

                        }

                        if (handyReport.HandyReportLogs.Count > 0)
                        {
                            var handyReportInsertResult = HandyReportLogModel.InsertHandyReportLog(registData.DatabaseName, handyReport.HandyReportLogs);
                        }

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, returnStoreAddressPostBack, "データの登録に失敗しました");
                    }

                    tran.Commit();
                    return (true, returnStoreAddressPostBack, "");
                }
            }

        }


    }
}
