using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using sumaken_api_agf.Models;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sumaken_api_agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfLanenoReadController : ControllerBase
    {
        // GET: api/<AgfLanenoRead>
        [HttpGet]
        [Route("GetLaneNo/{companyID}")]
        public async Task<IActionResult> GetLaneNo(int companyID, int depoCode)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var lanNos = await this.GetLaneNoData(databaseName, depoCode);
            return Ok(lanNos);
        }

        private async Task<List<string>> GetLaneNoData(string databaseName, int depoCode)
        {
            var laneNos = new List<string>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT [lane_no]
                             FROM [M_AGF_Lane]
                            ";
                var param = new
                {
                    DepoCode = depoCode
                };
                laneNos = (await connection.QueryAsync<string>(query, param)).ToList();
            }
            return laneNos;
        }

        // GET api/<AgfLanenoRead>/5
        [HttpGet]
        [Route("GetBinCode/{companyID}")]
        public async Task<IActionResult> GetBinCode(int companyID, int depoCode)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var agfBinCodeDatas = await this.GetBinCodeData(databaseName, depoCode);
            return Ok(agfBinCodeDatas);
        }

        private async Task<List<AGFBinCodeModel>> GetBinCodeData(string databaseName, int depoCode)
        {
            var agfBinCodeDatas = new List<AGFBinCodeModel>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                             SELECT
                                B.depo_code AS DepoCode,
                                   B.lane_no AS LaneNo,
                                   A.truck_bin_code AS TruckBinCode,
                                   A.truck_bin_name AS TruckBinName
                             FROM [M_AGF_TruckBin] AS A
                             LEFT JOIN [M_AGF_TruckBinLane] AS B
                             ON A.truck_bin_code = B.truck_bin_code
                             WHERE B.depo_code = @DepoCode
                            ";
                var param = new
                {
                    DepoCode = depoCode
                };
                agfBinCodeDatas = (await connection.QueryAsync<AGFBinCodeModel>(query, param)).ToList();
            }
            return agfBinCodeDatas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="depoCode">デポコード</param>
        /// <param name="settingFlag">セット方法が0：平積みの場合, セット方法が1：段積みの場合</param>
        /// <param name="laneNo"></param>
        /// <returns></returns>
        // GET api/<AgfLanenoRead>/5
        [HttpGet]
        [Route("GetLaneState/{companyID}")]
        public async Task<IActionResult> GetLaneState(int companyID, int depoCode, string settingFlag, string laneNo)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;
            var laneNoList = JsonConvert.DeserializeObject<List<string>>(laneNo);

            var laneStateDatas = await this.GetLaneStateData(databaseName, depoCode, settingFlag, laneNoList);
            return Ok(laneStateDatas);
        }

        private async Task<List<AGFLaneStateModel>> GetLaneStateData(string databaseName, int depoCode, string settingFlag, List<string> laneNo)
        {
            laneNo = laneNo.Where(x => string.IsNullOrWhiteSpace(x) != true).ToList();
            var strLanNo = "'" + string.Join("','", laneNo) + "'";
            var agfBinCodeDatas = new List<AGFLaneStateModel>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = string.Empty;
                if (settingFlag.Equals("0"))
                {
                    query = @$"
                            SELECT 
                              [depo_code] AS DepoCode,
                              [lane_no] AS LaneNo,
                              [lane_address] AS LaneAddress,
                              [change_address] ChangeAddress,
                              [sort_address] AS SortAddress,
                              [state] AS [State]
                            FROM [W_AGF_LaneState]
                             WHERE [depo_code] = @DepoCode
                                   AND ([lane_no] IN({strLanNo}))
                                   AND ([sort_address] <> 0)
                            ORDER BY [sort_address]
                            ";
                }
                else if (settingFlag.Equals("1"))
                {
                    query = @$"
                            SELECT  
                                [depo_code] AS DepoCode,
                                [lane_no] AS LaneNo,
                                [lane_address] AS LaneAddress,
                                [change_address] ChangeAddress,
                                [sort_address] AS SortAddress,
                                [state] AS [State]
                            FROM [W_AGF_LaneState]
                            WHERE [depo_code] = @DepoCode
                            AND ([lane_no] IN({strLanNo}))
                            ORDER BY [sort_address]
                            ";
                }
               
                var param = new
                {
                    DepoCode = depoCode
                };
                agfBinCodeDatas = (await connection.QueryAsync<AGFLaneStateModel>(query, param)).ToList();
            }
            return agfBinCodeDatas;
        }


        // POST api/<AgfLanenoRead>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgfLanenoRead>/5
       
    }
}
