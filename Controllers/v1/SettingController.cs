using Dapper;
using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using System.Data.SqlClient;
using static SakaguraAGFWebApi.Models.SettingModel;

namespace WarehousSakaguraAGFWebApieWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(SettingPostBackContent outPut)
        {
            return Ok(outPut);
        }

        // POST: api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody] SettingModel.SettingPostBody input)
        {
            //var input = new SettingModel.SettingPostBody();
            var createDatetime = DateTime.Now;
            var connectionString = "";

            //input = inputs[0];

            // 会社情報の取得
            var company = new CompanyModel.M_Company();
            var companys = CompanyModel.GetCompanyByCompanyCodeAndPassword(input.CompanyCode, input.CompanyPassword);
            if (companys.Count != 1) return Responce.ExBadRequest("会社情報の取得に失敗しました");
            if (String.IsNullOrEmpty(companys[0].DatabaseName)) return Responce.ExBadRequest("データベースの取得に失敗しました");
            company = companys[0];

            // ハンディユーザー情報の取得
            var handyUser = new HandyUserModel.M_HandyUser();
            var handyUsers = HandyUserModel.GetHandyUserByHandyUserCode(company.DatabaseName, input.HandyUserCode);
            if (handyUsers.Count != 1) return Responce.ExBadRequest("ユーザー情報の取得に失敗しました");
            handyUser = handyUsers[0];

            // デバイスの登録
            int handyDeviceUpdateCount = 0;
            connectionString = new GetConnectString(company.DatabaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"
                                        MERGE INTO D_HandyDevice AS A
                                        USING (SELECT @HandyUserID AS HandyUserID, @Device AS Device) AS B
                                        ON
                                        (
                                           A.HandyUserID = B.HandyUserID AND A.Device = B.Device
                                        )
                                        WHEN MATCHED THEN
                                            UPDATE SET
                                                 UpdateDate = @UpdateDate
                                                ,UpdateUserID = @UpdateUserID
                                        WHEN NOT MATCHED THEN
                                            INSERT
		                                    (
		                                       HandyUserID
		                                      ,Device
		                                      ,DeviceName
		                                      ,CreateDate
		                                      ,CreateUserID
		                                      ,UpdateDate
		                                      ,UpdateUserID
		                                    )
                                            VALUES
                                            (
		                                       @HandyUserID
		                                      ,@Device
		                                      ,@DeviceName
		                                      ,@CreateDate
		                                      ,@CreateUserID
		                                      ,@UpdateDate
		                                      ,@UpdateUserID
                                            )
                                    ;";
                        var param = new
                        {
                            handyUser.HandyUserID,
                            input.Device,
                            input.DeviceName,
                            CreateDate = createDatetime,
                            CreateUserID = handyUser.HandyUserID,
                            UpdateDate = createDatetime,
                            UpdateUserID = handyUser.HandyUserID
                        };
                        handyDeviceUpdateCount = connection.Execute(query, param, tran);

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Responce.ExServerError(ex);
                    }

                    if (handyDeviceUpdateCount != 1)
                    {
                        tran.Rollback();
                        return Responce.ExBadRequest("デバイス情報の更新に失敗しました");
                    }
                    else
                    {
                        tran.Commit();
                    }

                }
            }

            // 設定の登録成功
            var outPut = new SettingModel.SettingPostBackContent();
            outPut.CompanyID = company.CompanyID;
            outPut.HandyUserID = handyUser.HandyUserID;
            outPut.PasswordMode = handyUser.PasswordMode;

            //return CreatedAtAction(nameof(Get), new { companyID = outPut.CompanyID, handyUserID = outPut.HandyUserID, passwordMode = outPut.PasswordMode});
            //return CreatedAtAction(nameof(Get), new { companyID = outPut.CompanyID }, outPut);
            return CreatedAtAction(nameof(Get), outPut);
            //return Ok(outPut);
        }

    }
}
