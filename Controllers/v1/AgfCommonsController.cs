using Dapper;
using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using sumaken_api_agf.Commons;
using System.Data.SqlClient;
using technoleight_THandy.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sumaken_api_agf.Controllers.v1
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

        // GET: api/<AgfCommonsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AgfCommonsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgfCommonsController>
        [HttpPost()]
        [Route("ScanRecord/{companyID}")]
        public async Task<IActionResult> ScanRecord(int companyID, [FromBody] List<AGFScanRecordModel> scanRecords)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;
            await SaveScanRecord(databaseName, scanRecords);
            return Ok();
        }

        private async Task<int> SaveScanRecord(string databaseName, List<AGFScanRecordModel> scanRecords)
        {
            var affectedRows = 0;
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
                            var result = await connection.ExecuteAsync(query, param, tran);
                            if(result > 0) { affectedRows =  affectedRows + result; }
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
            return affectedRows;
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
