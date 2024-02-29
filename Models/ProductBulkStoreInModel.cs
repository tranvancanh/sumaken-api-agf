using Dapper;
using SakaguraAGFWebApi.Commons;
using System.Data.SqlClient;

namespace SakaguraAGFWebApi.Models
{
    public class ProductBulkStoreInModel
    {
        public class ProductBulkStoreIn
        {
            public int ProductID { get; set; }
            public string ProductCode { get; set; } = string.Empty;
        }

        public static List<ProductBulkStoreIn> GetProductBulkStoreIn(string databaseName, int depoID)
        {
            var selectList = new List<ProductBulkStoreIn>();

            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                             ProductID
                                            ,ProductCode
                                        FROM M_Product
                                        WHERE 1=1
                                            AND DepoID = @DepoID
                                            AND NotUseFlag = @NotUseFlag
                                            AND BulkStoreInFlag = @BulkStoreInFlag
                                                ";
                    var param = new
                    {
                        DepoID = depoID,
                        NotUseFlag = 0,
                        BulkStoreInFlag = 1
                    };
                    selectList = connection.Query<ProductBulkStoreIn>(query, param).ToList();

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
