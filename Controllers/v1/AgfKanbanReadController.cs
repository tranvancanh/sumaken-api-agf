using Dapper;
using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using System.Data.SqlClient;
using technoleight_THandy.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sumaken_Api_Agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfKanbanReadController : ControllerBase
    {
        // GET: api/<AgfKanbanRead>
        [HttpGet()]
        [Route("AgfKanbanCheckSagyouSha/{companyID}")]
        public async Task<IActionResult> AgfKanbanCheckSagyouSha(int companyID, int depoCode, string customerCode, string ukeire)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var agfShukaKanbanDatas = await GetAGFShukaKanbanDatas(databaseName, depoCode, customerCode, ukeire);
            return Ok(agfShukaKanbanDatas);
        }

        private async Task<List<AGFShukaKanbanDataModel>> GetAGFShukaKanbanDatas(string databaseName, int depoCode, string customerCode, string ukeire)
        {
            var agfShukaKanbanDatas = new List<AGFShukaKanbanDataModel>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT 
                                A.depo_code AS DepoCode,
                                A.customer_code AS CustomerCode,
                                A.final_delivery_place AS Ukeire,
                                A.truck_bin_code AS Bin,
                                B.truck_bin_code AS SagyoShaCode,
                                B.truck_bin_name AS SagyoShaName
                            FROM [M_AGF_DestinationBin] AS A
                            LEFT JOIN [M_AGF_TruckBin] AS B
                            ON A.[truck_bin_code] = B.truck_bin_code
                            WHERE A.depo_code = @DepoCode
                            AND A.customer_code = @CustomerCode
                            AND A.final_delivery_place = @Ukeire
                            AND B.truck_bin_code IS Not NULL
                            ";
                var param = new
                {
                    DepoCode = depoCode,
                    CustomerCode = customerCode,
                    Ukeire = ukeire
                };
                agfShukaKanbanDatas = (await connection.QueryAsync<AGFShukaKanbanDataModel>(query, param)).ToList();
            }
            return agfShukaKanbanDatas;
        }

        // GET api/<AgfKanbanRead>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgfKanbanRead>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AgfKanbanRead>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AgfKanbanRead>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
