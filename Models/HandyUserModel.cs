using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class HandyUserModel
    {
        public class M_HandyUser
        {
            public int HandyUserID { get; set; }
            public string HandyUserCode { get; set; } = string.Empty;
            public string HandyUserPassword { get; set; } = string.Empty;
            public string HandyUserName { get; set; } = string.Empty;
            public int AdministratorFlag { get; set; }
            public int DefaultHandyPageID { get; set; }
            public int PasswordMode { get; set; }
        }

        public static List<M_HandyUser> GetHandyUserByHandyUserCode(string databaseName, string handyUserCode)
        {
            var handyUsers = new List<M_HandyUser>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                               HandyUserID,
                                               HandyUserCode,
                                               HandyUserName,
                                               AdministratorFlag,
                                               DefaultHandyPageID,
                                               PasswordMode
                                        FROM M_HandyUser
                                        WHERE 1=1
                                            AND HandyUserCode = @HandyUserCode
                                            AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        HandyUserCode = handyUserCode,
                        NotUseFlag = 0
                    };
                    handyUsers = connection.Query<M_HandyUser>(query, param).ToList();
                    return handyUsers;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static List<M_HandyUser> GetHandyUserByHandyUserIDAndCode(string databaseName, int handyUserID, string handyUserCode)
        {
            var handyUsers = new List<M_HandyUser>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                               HandyUserID,
                                               HandyUserCode,
                                               HandyUserName,
                                               AdministratorFlag,
                                               DefaultHandyPageID,
                                               PasswordMode
                                        FROM M_HandyUser
                                        WHERE 1=1
                                            AND HandyUserID = @HandyUserID
                                            AND HandyUserCode = @HandyUserCode
                                            AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        HandyUserID = handyUserID,
                        HandyUserCode = handyUserCode,
                        NotUseFlag = 0
                    };
                    handyUsers = connection.Query<M_HandyUser>(query, param).ToList();
                    return handyUsers;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static List<M_HandyUser> GetHandyUserByHandyUserIDAndCodeAndPassword(string databaseName, int handyUserID, string handyUserCode, string handyUserPassword)
        {
            var handyUsers = new List<M_HandyUser>();
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = @"
                                        SELECT
                                               HandyUserID,
                                               HandyUserCode,
                                               HandyUserName,
                                               AdministratorFlag,
                                               DefaultHandyPageID,
                                               PasswordMode
                                        FROM M_HandyUser
                                        WHERE 1=1
                                            AND HandyUserID = @HandyUserID
                                            AND HandyUserCode = @HandyUserCode
                                            AND HandyUserPassword = @HandyUserPassword
                                            AND NotUseFlag = @NotUseFlag
                                                ";
                    var param = new
                    {
                        HandyUserID = handyUserID,
                        HandyUserCode = handyUserCode,
                        HandyUserPassword = handyUserPassword,
                        NotUseFlag = 0
                    };
                    handyUsers = connection.Query<M_HandyUser>(query, param).ToList();
                    return handyUsers;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }

}
