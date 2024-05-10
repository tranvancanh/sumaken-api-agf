using Dapper;
using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using sumaken_api_agf.Commons;
using System.Data.SqlClient;
using System.Diagnostics;
using technoleight_THandy.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sumaken_Api_Agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfCommonsController : ControllerBase
    {
        private readonly ILogger<AgfCommonsController> _logger;
        public AgfCommonsController(ILogger<AgfCommonsController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Nlog is started to AGF共通処理開始");
        }

        // POST api/<AgfCommonsController>
        [HttpPost()]
        [Route("ScanRecord/{companyID}")]
        public async Task<IActionResult> ScanRecord(int companyID, [FromBody] List<AGFScanRecordModel> scanRecords)
        {
            try
            {
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
                var databaseName = companys[0].DatabaseName;
                var agf_ScanRecordID = await SaveScanRecord(databaseName, scanRecords);
                return Ok(agf_ScanRecordID);
            }
            catch (Exception ex)
            {
                _logger.LogError("ScanRecordの保存が発生しました。");
                _logger.LogError("Message   ：   " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<long> SaveScanRecord(string databaseName, List<AGFScanRecordModel> scanRecords)
        {
            var agf_ScanRecordID = 0L;
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        foreach(var item in scanRecords)
                        {
                            var query = @"
                            INSERT INTO [D_AGF_ScanRecord] 
                            ([DepoID],[HandyUserID],[HandyOperationClass],[HandyOperationMessage],[Device],[HandyPageID],[ScanString1],[ScanString2],[ScanString3],[ScanTime],[Latitude],[Longitude],[CreateDate])
                             OUTPUT INSERTED.AGF_ScanRecordID
                            VALUES (@DepoID,@HandyUserID,@HandyOperationClass,@HandyOperationMessage,@Device,@HandyPageID,@ScanString1,@ScanString2,@ScanString3,@ScanTime,@Latitude,@Longitude,@CreateDate)
                            ;";
                            var param = new
                            {
                                DepoID = item.DepoID,
                                HandyUserID = item.HandyUserID,
                                HandyOperationClass = item.HandyOperationClass,
                                HandyOperationMessage  = item.HandyOperationMessage,
                                Device = item.Device,
                                HandyPageID = item.HandyPageID,
                                ScanString1 = item.ScanString1,
                                ScanString2 = item.ScanString2,
                                ScanString3 = item.ScanString3,
                                ScanTime = item.ScanTime,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude,
                                CreateDate = DateTime.Now
                            };
                            agf_ScanRecordID = await connection.QuerySingleAsync<long>(query, param, tran);
                        }

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            return agf_ScanRecordID;
        }

        // POST api/<AgfCommonsController>
        [HttpPost()]
        [Route("UpdateScanRecordByID/{companyID}")]
        public async Task<IActionResult> UpdateScanRecordByID(int companyID, AGFScanRecordModel scanRecordModel)
        {
            try
            {
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");

                var databaseName = companys[0].DatabaseName;
                var agf_ScanRecordID = await UpdateScanRecord(databaseName, scanRecordModel);
                return Ok(agf_ScanRecordID);
            }
            catch (Exception ex)
            {
                _logger.LogError("ScanRecordの更新が発生しました。");
                _logger.LogError("Message   ：   " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<int> UpdateScanRecord (string databaseName, AGFScanRecordModel scanRecordModel)
        {
            var agfScanRecordID = scanRecordModel.AGF_ScanRecordID;
            var handyOperationClass = scanRecordModel.HandyOperationClass; 
            var handyOperationMessage = scanRecordModel.HandyOperationMessage;

            var result = 0;
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"
                                    UPDATE [D_AGF_ScanRecord]
                                    SET HandyOperationClass = @HandyOperationClass, HandyOperationMessage = @HandyOperationMessage
                                    WHERE [AGF_ScanRecordID] = @AGF_ScanRecordID
                                    ";
                        var param = new
                        {
                            AGF_ScanRecordID = agfScanRecordID,
                            HandyOperationClass = handyOperationClass,
                            HandyOperationMessage = handyOperationMessage
                        };
                        result = await connection.ExecuteAsync(query, param, tran);

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            return result;
        }

        [HttpGet()]
        [Route("CheckSaveCSVPath/{companyID}")]
        public async Task<IActionResult> CheckSaveCSVPath(int companyID)
        {
            var startTime = DateTime.Now;
            try
            {
                _logger.LogInformation("AGF共有フォルダ処理は開始");

                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
                var databaseName = companys[0].DatabaseName;
                var companyCode = companys[0].CompanyCode;

                var result = await NetworkShareAccesser.CheckAccessServerOrSharedResource(databaseName, companyCode);
                if(result.Level == NetworkShareAccesser.Level.Infor)
                    _logger.LogInformation(result.Mess);
                else
                    _logger.LogError(result.Mess);
                var endTime = DateTime.Now;
                var elapsed = endTime - startTime;
                var completeTime = elapsed.ToString(@"hh\:mm\:ss\.ffff");
                _logger.LogInformation("AGF共有フォルダ処理は正常終了");
                _logger.LogInformation("AGF共有フォルダ時間は: " + completeTime);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("AGF共有フォルダはエラーが発生しました。");
                _logger.LogError("Message   ：   " + ex.Message);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                var endTime = DateTime.Now;
                var elapsed = endTime - startTime;
                var completeTime = elapsed.ToString(@"hh\:mm\:ss\.ffff");
                _logger.LogInformation("時間かかるのは: " + completeTime);
            }

        }

    }
}
