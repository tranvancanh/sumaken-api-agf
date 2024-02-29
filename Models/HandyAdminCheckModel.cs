using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class HandyAdminCheckModel
    {
        public class HandyAdminCheckPostBody
        {
            [Required]
            public int CompanyID { get; set; }
            [Required]
            public string HandyAdminPassword { get; set; } = "";
        }

        public static int HandyAdminCheck(int companyID, string password)
        {
            var isHandyAdmin = 0;
            var connectionString = new GetMasterConnectString().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                            Count(*)
                                        FROM M_Company
                                        WHERE (1=1)
                                            AND CompanyID = @CompanyID
                                            AND HandyAdminPassword = @HandyAdminPassword
                                                ";
                    var param = new
                    {
                        CompanyID = companyID,
                        HandyAdminPassword = password
                    };
                    isHandyAdmin = connection.ExecuteScalar<int>(query, param);
                    return isHandyAdmin;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }
}
