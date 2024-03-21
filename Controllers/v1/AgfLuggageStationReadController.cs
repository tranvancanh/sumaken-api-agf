using Dapper;
using Microsoft.AspNetCore.Mvc;
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
    public class AgfLuggageStationReadController : ControllerBase
    {
        // GET: api/<AgfLuggageStationRead>
        [HttpGet("{companyID}")]
        public async Task<IActionResult> Get(int companyID, int depoID)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var agfLuggageStation = await this.GetAGFLuggageStation(databaseName, depoID);
            return Ok(agfLuggageStation);
        }

        [HttpGet()]
        [Route("AgfLuggageStationCheck/{companyID}")]
        public async Task<IActionResult> AgfLuggageStationCheck(int companyID, int depoCode, string luggageStation)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;
            var agfLuggageStationCheck = await this.CheckAgfLuggageStation(databaseName, depoCode, luggageStation);
            if(agfLuggageStationCheck.Any())
                return Ok(true);
            return NotFound("QRコードは「荷取STマスター」に存在しません。");
        }

        private async Task<List<AGFLuggageStation>> CheckAgfLuggageStation(string databaseName, int depoCode, string luggageStation)
        {
            var agfLuggageStation = new List<AGFLuggageStation>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT [depo_code] AS DepoCode
                                    ,[luggage_station] AS LuggageStation
                                    ,[change_luggage_station] AS ChangeLuggageStation
                                FROM [M_AGF_LuggageStation]
                            WHERE (1=1)
                            AND [depo_code] = @DepoCode
                            AND [luggage_station] = @LuggageStation
                            ";
                var param = new
                {
                    DepoCode = depoCode,
                    LuggageStation = luggageStation
                };
                agfLuggageStation = (await connection.QueryAsync<AGFLuggageStation>(query, param)).ToList();
            }
            return agfLuggageStation;
        }

        private async Task<List<AGFLuggageStation>> GetAGFLuggageStation(string databaseName, int depoCode)
        {
            var agfLuggageStation = new List<AGFLuggageStation>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT [depo_code] AS DepoCode
                                    ,[luggage_station] AS LuggageStation
                                    ,[change_luggage_station] AS ChangeLuggageStation
                                FROM [M_AGF_LuggageStation]
                            WHERE (1=1)
                            AND [depo_code] = @DepoCode
                            ";
                var param = new
                {
                    DepoCode = depoCode
                };
                agfLuggageStation = (await connection.QueryAsync<AGFLuggageStation>(query, param)).ToList();
            }
            return agfLuggageStation;
        }

        // GET api/sample
        [HttpGet]
        [Route("sample")] // Route attribute defines the endpoint URL
        public ActionResult<string> sample()
        {
            // Your logic to retrieve sample data
            var sampleData = "This is sample data.";
            return Ok(sampleData);
        }

        // POST api/<AgfLuggageStationRead>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgfLuggageStationRead>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AgfLuggageStationRead>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
