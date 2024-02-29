using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using static SakaguraAGFWebApi.Models.ReceiveDeleteModel;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReceiveDeleteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(ReceiveDeletePostBackContent outPut)
        {
            return Ok(outPut);
        }

        // POST: api/<controller>
        [HttpPost("{companyID}")]
        public IActionResult Post(int companyID, [FromBody] ReceiveDeletePostBody input)
        {
            // 会社情報の取得
            var companys = new List<CompanyModel.M_Company>();
            var company = new CompanyModel.M_Company();
            var databaseName = "";
            try
            {
                companys = CompanyModel.GetCompanyByCompanyID(companyID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }
            if (companys.Count != 1) return Responce.ExBadRequest("会社情報の取得に失敗しました");
            if (String.IsNullOrEmpty(companys[0].DatabaseName)) return Responce.ExBadRequest("データベースの取得に失敗しました");
            company = companys[0];
            databaseName = companys[0].DatabaseName;

            int deleteReceiveDataCount = 0;
            try
            {
                var connectionString = new GetConnectString(databaseName).ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                        try
                        {
                            var query = 
                                            $@"

                                            UPDATE D_Receive SET
                                                DeleteFlag = @DeleteFlag,
                                                UpdateDate = @UpdateDate,
                                                UpdateUserID = @UpdateUserID
                                            WHERE ReceiveDate >= @ReceiveDate;

                                            UPDATE D_StoreIn SET
                                                DeleteFlag = @DeleteFlag,
                                                UpdateDate = @UpdateDate,
                                                UpdateUserID = @UpdateUserID
                                            WHERE StoreInDate >= @ReceiveDate;

                                            ";
                        var param = new
                        {
                            ReceiveDate = input.DeleteReceiveStartDate,
                            DeleteFlag = 1,
                            UpdateDate = DateTime.Now,
                            UpdateUserID = input.UserID
                        };
                        deleteReceiveDataCount = connection.Execute(query, param);

                        }
                        catch (Exception ex)
                        {
                            return Responce.ExServerError(ex);
                        }
                }
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            // 設定の登録成功
            var outPut = new ReceiveDeleteModel.ReceiveDeletePostBackContent();
            outPut.DeleteReceiveDataCount = deleteReceiveDataCount;

            return CreatedAtAction(nameof(Get), outPut);

        }

      }
}
