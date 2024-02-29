using Dapper;
using SakaguraAGFWebApi.Commons;
using System.Data.SqlClient;

namespace SakaguraAGFWebApi.Models
{
    public class StoreInConstraintModel
    {
        public class M_StoreInConstraint
        {
            public string NextProcess1 { get; set; } = string.Empty;
            public string NextProcess2 { get; set; } = string.Empty;
        }

        public static List<M_StoreInConstraint> GetStoreInConstraint(string databaseName, int depoID)
        {
            var selectList = new List<M_StoreInConstraint>();

            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                               NextProcess1
                                              ,NextProcess2
                                        FROM M_StoreInConstraint
                                        WHERE 1=1
                                            AND DepoID = @DepoID
                                            AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        DepoID = depoID,
                        NotUseFlag = 0
                    };
                    selectList = connection.Query<M_StoreInConstraint>(query, param).ToList();

                    return selectList;
                }
                catch (Exception e)
                {
                    throw;
                }
            }

        }

    }

}
