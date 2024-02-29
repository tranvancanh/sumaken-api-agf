using Dapper;
using SakaguraAGFWebApi.Commons;
using System.Data.SqlClient;

namespace SakaguraAGFWebApi.Models
{
    public class TemporaryStoreAddressModel
    {
        public class M_TemporaryStoreAddress
        {
            public int TemporaryStoreAddressID { get; set; }
            public string TemporaryStoreAddress1 { get; set; } = string.Empty;
            public string TemporaryStoreAddress2 { get; set; } = string.Empty;
        }

        public static List<M_TemporaryStoreAddress> GetTemporaryStoreAddress(string databaseName, int depoID)
        {
            var selectList = new List<M_TemporaryStoreAddress>();

            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                             TemporaryStoreAddressID
                                            ,TemporaryStoreAddress2
                                            ,TemporaryStoreAddress1
                                        FROM M_TemporaryStoreAddress
                                        WHERE 1=1
                                            AND DepoID = @DepoID
                                                ";
                    var param = new
                    {
                        DepoID = depoID
                    };
                    selectList = connection.Query<M_TemporaryStoreAddress>(query, param).ToList();

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
