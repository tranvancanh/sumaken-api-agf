using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using sumaken_api_agf.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using technoleight_THandy.Models;
using static SakaguraAGFWebApi.Models.HandyReportLogModel;
using static SakaguraAGFWebApi.Models.QrcodeModel;
using static SakaguraAGFWebApi.Models.ReceiveModel;

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

            var laneStateDatas = new List<AGFLaneStateModel>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var laneNoList = JsonConvert.DeserializeObject<List<string>>(laneNo);
                var laneNoListAfter = laneNoList.Where(x => string.IsNullOrWhiteSpace(x) != true).ToList();
                laneStateDatas = await this.GetLaneStateData(depoCode, settingFlag, laneNoListAfter, connection);

                if (!laneStateDatas.Any())
                    return NotFound();
            }
            return Ok(laneStateDatas.First());

        }

        private async Task<List<AGFLaneStateModel>> GetLaneStateData(int depoCode, string settingFlag, List<string> laneNo, SqlConnection sqlConnection = null, SqlTransaction sqlTransaction = null)
        {
            var strLanNo = "'" + string.Join("','", laneNo) + "'";
            var laneStateDatas = new List<AGFLaneStateModel>();
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
                        AND [state] = '0'
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
                        AND [state] = '0'
                        ORDER BY [change_address]
                        ";
            }

            var param = new
            {
                DepoCode = depoCode
            };
            laneStateDatas = (await sqlConnection.QueryAsync<AGFLaneStateModel>(query, param, sqlTransaction)).ToList();
          
            return laneStateDatas;
        }

        // POST api/<AgfLanenoRead>
        [HttpPost]
        [Route("UpdateStateAndCreateCSV/{companyID}")]
        public async Task<IActionResult> UpdateStateAndCreateCSV(int companyID, object objApi)
        {
            if(objApi == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            try
            {
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
                var databaseName = companys[0].DatabaseName;

                var objApiStr = objApi.ToString();
                dynamic data = JsonConvert.DeserializeObject<dynamic>(objApiStr);
                int depoCode = data.DepoCode;
                string address1 = data.Address1;
                string shukaKanban = data.ShukaKanban;
                string settingFlag = data.SettingFlag;
                string laneNo = data.LaneNo;
                string laneStates = data.LaneStates;
                string address3 = data.Address3;
                string handyUserID = data.HandyUserID;
                string handyUserCode = data.HandyUserCode;

                var laneNoList = JsonConvert.DeserializeObject<List<string>>(laneNo);
                var laneNoListAfter = laneNoList.Where(x => string.IsNullOrWhiteSpace(x) != true).ToList();

                var shukaKanbanData = JsonConvert.DeserializeObject<List<AGFShukaKanbanDataModel>>(shukaKanban).First();
                var result = await this.AGFStateUpdate(databaseName, depoCode, settingFlag, laneNoListAfter, shukaKanbanData, handyUserID, address1);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
        }

        private async Task<int> AGFStateUpdate(string databaseName, int depoCode, string settingFlag, List<string> laneNos, AGFShukaKanbanDataModel shukaKanbanData, string handyUserID, string luggageStation)
        {
            var affectedRows = 0;
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var agfStates = await this.GetLaneStateData(depoCode, settingFlag, laneNos, connection, transaction);
                        if (!agfStates.Any())
                        {
                            throw new Exception("出荷レーンがいっぱいの場合もエラー");
                        }
                        AGFLaneStateModel agfLaneState = null;
                        for (var i = 0; i < agfStates.Count; i++)
                        {
                            var item = agfStates[i];
                            var updateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                            //平積み
                            if (settingFlag.Equals("0"))
                            {
                                //stateは0なら1荷物ありを立てる。1がすでに立っている場合は次の番号に1を立てる
                                var SQL_UPDATE = @$"
                                            UPDATE [W_AGF_LaneState]
                                            SET [state] = '1',
                                                [update_date] = @UpdateTime,
                                                [update_user_id] = @UpdateUserID
                                            WHERE [depo_code] = @DepoCode
                                            AND [lane_no] = @LaneNo
                                            AND [lane_address] = @LaneAddress
                                            AND [state] = '0'
                                            ";
                                var param = new
                                {
                                    DepoCode = item.DepoCode,
                                    LaneNo = item.LaneNo,
                                    LaneAddress = item.LaneAddress,
                                    UpdateTime = updateTime,
                                    UpdateUserID = handyUserID
                                };
                                var updateRows = await connection.ExecuteAsync(SQL_UPDATE, param, transaction);
                                if (updateRows > 0)
                                {
                                    affectedRows = affectedRows + updateRows;
                                    agfLaneState = item;
                                    break;
                                }
                            }
                            //段積み
                            else if (settingFlag.Equals("1"))
                            {
                                //stateは0なら1荷物ありを立てる。1がすでに立っている場合は次の番号に1を立てる
                                var changeAdress = Convert.ToString(item.ChangeAddress);
                                var lastItem = changeAdress.Select(c => c.ToString()).ToList().Last();
                                var SQL_UPDATE1 = @$"
                                            UPDATE [W_AGF_LaneState]
                                            SET [state] = '1',
                                                [update_date] = @UpdateTime,
                                                [update_user_id] = @UpdateUserID
                                            WHERE [depo_code] = @DepoCode
                                            AND [lane_no] = @LaneNo
                                            AND [lane_address] = @LaneAddress
                                            AND [state] = '0'
                                            ";
                                var param1 = new
                                {
                                    DepoCode = item.DepoCode,
                                    LaneNo = item.LaneNo,
                                    LaneAddress = item.LaneAddress,
                                    UpdateTime = updateTime,
                                    UpdateUserID = handyUserID
                                };
                                var updateRows1 = await connection.ExecuteAsync(SQL_UPDATE1, param1, transaction);
                                if (updateRows1 > 0)
                                {
                                    affectedRows = affectedRows + updateRows1;
                                    /*
                                     今回1を立てて位置が下段の場合（change_addressの末尾が1の場合）
                                     部品番号をチェックしてM_AGF_StackingNGにこの部品登録がある場合
                                     次の上段のレーンの状態=2禁止を入れる
                                    */
                                    if (lastItem.Equals("1") && (i < (agfStates.Count-1)))
                                    {
                                        //部品番号をチェックしてM_AGF_StackingNGにこの部品登録がある場合
                                        var check = await this.CheckExistHinban(depoCode, shukaKanbanData.Hinban, connection, transaction);
                                        if (check)
                                        {
                                            //次の上段のレーンの状態=2禁止を入れる
                                            var nextItem = agfStates[i + 1];
                                            var SQL_UPDATE2 = @$"
                                                                UPDATE [W_AGF_LaneState]
                                                                SET [state] = '2'
                                                                WHERE [depo_code] = @DepoCode
                                                                AND [lane_no] = @LaneNo
                                                                AND [lane_address] = @LaneAddress
                                                               ";
                                            var param2 = new
                                            {
                                                DepoCode = nextItem.DepoCode,
                                                LaneNo = nextItem.LaneNo,
                                                LaneAddress = nextItem.LaneAddress
                                            };
                                            var updateRows2 = await connection.ExecuteAsync(SQL_UPDATE2, param2, transaction);
                                            affectedRows = affectedRows + updateRows2;
                                        }
                                    }
                                    agfLaneState = item;
                                    break;
                                }
                            }
                        }

                        if(agfLaneState == null)
                        {
                            throw new Exception("出荷レーンがいっぱいの場合もエラー");
                        }
                        var createTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                   
                        //行先名称を存在するチェック
                        var SQL_SELECT_DESTINATION = @$"
                                                        SELECT A.destination
                                                            FROM[M_AGF_Destination] AS A
                                                            LEFT JOIN [M_AGF_DestinationBin] AS B 
                                                            ON A.customer_code = B.customer_code AND A.final_delivery_place = B.final_delivery_place
  
                                                            WHERE B.[depo_code] is not null
                                                            AND A.[customer_code] = @CustomerCode
                                                            AND A.[final_delivery_place] = @FinalDeliveryPlace
                                                            AND B.[depo_code] = @DepoCode
                                                            AND B.[truck_bin_code] = @TruckBinCode
                                                    ";
                        var param_select_destination = new
                        {
                            CustomerCode = shukaKanbanData.CustomerCode,
                            FinalDeliveryPlace = shukaKanbanData.Ukeire,
                            DepoCode = depoCode,
                            TruckBinCode = shukaKanbanData.SagyoShaCode
                        };
                        DataTable table = new DataTable();
                        var reader = await connection.ExecuteReaderAsync(SQL_SELECT_DESTINATION, param_select_destination, transaction);
                        table.Load(reader);

                        if(table.Rows.Count > 0)
                        {
                            var destination = Convert.ToString(table.Rows[0]["destination"]);
                            //A_AGF_Motionテーブルに書き込みを行う
                            var SQL_AGF_Motion_Insert = @$"
                                                    INSERT INTO [A_AGF_Motion] 
                                                    (
                                                        [depo_code],
                                                        [motion_date],
                                                        [product_code],
                                                        [luggage_station],
                                                        [lane_no],
                                                        [lane_address],
                                                        [truck_bin_name],
                                                        [customer_code],
                                                        [final_delivery_place],
                                                        [destination],
                                                        [create_date],
                                                        [create_user_id]
                                                    )
                                                    VALUES 
                                                    (
                                                        @DepoCode,
                                                        @MotionDate,
                                                        @ProductCode,
                                                        @LuggageStation,
                                                        @LaneNo,
                                                        @LaneAddress,
                                                        @TruckBinName,
                                                        @CustomerCode,
                                                        @FinalDeliveryPlace,
                                                        @Destination,
                                                        @CreateDate,
                                                        @CreateUserId
                                                    )
                                                    ";
                            var agf_Monitor_Param = new
                            {
                                DepoCode = depoCode,
                                MotionDate = DateTime.Now.ToString("yyyy/MM/dd"),
                                ProductCode = shukaKanbanData.Hinban,
                                LuggageStation = luggageStation,
                                LaneNo = agfLaneState.LaneNo,
                                LaneAddress = agfLaneState.LaneAddress,
                                TruckBinName = shukaKanbanData.SagyoShaName,
                                CustomerCode = shukaKanbanData.TokuiSakiCode,
                                FinalDeliveryPlace = shukaKanbanData.Ukeire,
                                Destination = destination,
                                CreateDate = createTime,
                                CreateUserId = handyUserID
                            };

                            var insertRows = await connection.ExecuteAsync(SQL_AGF_Motion_Insert, agf_Monitor_Param, transaction);
                            affectedRows = affectedRows + insertRows;
                        }
                        else
                        {
                            throw new Exception("行先名称が存在していません");
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return affectedRows;
        }

       
        private async Task<bool> CheckExistHinban(int depoCode, string productCode, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            var SQL_CHECK = @$"
                                SELECT Count(*) AS [TotalCount]
                                FROM [M_AGF_StackingNG]
                                WHERE [depo_code] = @DepoCode
                                AND [product_code] = @ProductCode
                             ";
            var param = new
            {
                DepoCode = depoCode,
                ProductCode = productCode
            };
            var count = (int)(await connection.ExecuteScalarAsync(SQL_CHECK, param, transaction));
            if(count > 0)
                return true;
            return false;
        }

        

        // PUT api/<AgfLanenoRead>/5
       
    }
}
