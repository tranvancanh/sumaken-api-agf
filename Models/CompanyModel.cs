using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class CompanyModel
    {
        public class M_Company
        {
            public int CompanyID { get; set; }
            public string CompanyCode { get; set; } = string.Empty;
            public string CompanyName { get; set; } = string.Empty;
            public string DatabaseName { get; set; } = string.Empty;
            public decimal HandyAppMinVersion { get; set; }
        }

        public static List<M_Company> GetCompanyByCompanyID(int companyID)
        {
            var companys = new List<M_Company>();
            var connectionString = new GetMasterConnectString().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                            CompanyID, CompanyCode, CompanyName, DatabaseName, HandyAppMinVersion
                                        FROM M_Company
                                        WHERE (1=1)
                                        AND CompanyID = @CompanyID
                                        AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        CompanyID = companyID,
                        NotUseFlag = 0
                    };
                    companys = connection.Query<M_Company>(query, param).ToList();
                    return companys;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static List<M_Company> GetCompanyByCompanyCodeAndPassword(string companyCode, string companyPassword)
        {
            var companys = new List<M_Company>();
            var connectionString = new GetMasterConnectString().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                            CompanyID, CompanyCode, CompanyName, DatabaseName
                                        FROM M_Company
                                        WHERE 1=1
                                            AND CompanyCode = @CompanyCode
                                            AND CompanyPassword = @CompanyPassword
                                            AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        CompanyCode = companyCode,
                        CompanyPassword = companyPassword,
                        NotUseFlag = 0
                    };
                    companys = connection.Query<M_Company>(query, param).ToList();
                    return companys;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }

}
