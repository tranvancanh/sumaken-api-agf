using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // POST: api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody] LoginModel.LoginPostBody input)
        {
            // 会社情報の取得
            var companys = new List<CompanyModel.M_Company>();
            var company = new CompanyModel.M_Company();
            var databaseName = "";
            try
            {
                companys = CompanyModel.GetCompanyByCompanyID(input.CompanyID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }
            if (companys.Count != 1) return Responce.ExBadRequest("会社情報の取得に失敗しました");
            if (String.IsNullOrEmpty(companys[0].DatabaseName)) return Responce.ExBadRequest("データベースの取得に失敗しました");
            company = companys[0];
            databaseName = companys[0].DatabaseName;

            if (company.HandyAppMinVersion > input.HandyAppVersion) return Responce.ExBadRequest("アプリを更新してください");

            // ユーザー情報の取得
            var handyUser = new HandyUserModel.M_HandyUser();
            var handyUsers = new List<HandyUserModel.M_HandyUser>();
            try
            {
                if (input.PasswordMode == 0)
                {
                    // ハンディユーザーパスワード不要
                    handyUsers = HandyUserModel.GetHandyUserByHandyUserIDAndCode(databaseName, input.HandyUserID, input.HandyUserCode);
                }
                else
                {
                    handyUsers = HandyUserModel.GetHandyUserByHandyUserIDAndCodeAndPassword(databaseName, input.HandyUserID, input.HandyUserCode, input.HandyUserPassword);
                }
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }
            if (handyUsers.Count != 1) return Responce.ExBadRequest("ログインに失敗しました");
            handyUser = handyUsers[0];

            // デポ情報の取得
            var depo = new DepoModel.M_Depo();
            var depos = new List<DepoModel.M_Depo>();
            try
            {
                depos = DepoModel.GetDepoByHandyUserID(databaseName, input.HandyUserID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }
            if (depos.Count != 1) return Responce.ExBadRequest("デポ情報の取得に失敗しました");
            depo = depos[0];

            // デバイスチェック
            // 設定で登録した最新のデバイスと一致しているか
            var latestDevice = "";
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    var query = @"
                                        SELECT
                                           Device
                                        FROM D_HandyDevice 
                                        WHERE 1=1
                                            AND HandyUserID = @HandyUserID
                                        ORDER BY UpdateDate Desc
                                                ";
                    var param = new
                    {
                        input.HandyUserID
                    };
                    latestDevice = connection.QueryFirstOrDefault<string>(query, param);
                }
                catch (Exception ex)
                {
                    return Responce.ExServerError(ex);
                }
                if (latestDevice != input.Device) return Responce.ExBadRequest("登録されたデバイスと一致しません");
            }

            // ログイン成功
            var outPut = new LoginModel.LoginPostBackContent();
            outPut.CompanyName = company.CompanyName;
            outPut.HandyUserName = handyUser.HandyUserName;
            outPut.AdministratorFlag = handyUser.AdministratorFlag;
            outPut.DefaultHandyPageID = handyUser.DefaultHandyPageID;
            outPut.DepoID = depo.DepoID;
            outPut.DepoCode = depo.DepoCode;
            outPut.DepoName = depo.DepoName;
            return Ok(outPut);

        }

      }
}
