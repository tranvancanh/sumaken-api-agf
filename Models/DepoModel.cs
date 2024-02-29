using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class DepoModel
    {
        public class M_Depo
        {
            public int DepoID { get; set; }
            public string DepoCode { get; set; } = string.Empty;
            public string DepoName { get; set; } = string.Empty;
        }

        public static List<M_Depo> GetDepoByHandyUserID(string databaseName, int handyUserID)
        {
            var depos = new List<DepoModel.M_Depo>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                    SELECT
                                        A.DepoID,
                                        B.DepoCode,
                                        B.DepoName
                                    FROM M_HandyUser AS A
                                    LEFT OUTER JOIN M_Depo AS B ON A.DepoID = B.DepoID
                                    WHERE (1=1)
                                        AND A.HandyUserID = @HandyUserID
                                        AND B.NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        HandyUserID = handyUserID,
                        NotUseFlag = 0
                    };
                    depos = connection.Query<M_Depo>(query, param).ToList();
                    return depos;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }

}
