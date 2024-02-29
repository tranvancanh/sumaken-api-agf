using Dapper;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Models
{
    public class HandyPageModel
    {
        public class M_HandyPage
        {
            public int HandyPageNumber { get; set; }
            public int HandyPageID { get; set; }
            public string HandyPageName { get; set; } = string.Empty;
        }

        public static List<M_HandyPage> GetHandyPage(string databaseName, int depoID, int administratorFlag, int handyUserID)
        {
            var selectList = new List<M_HandyPage>();

            // ハンディーユーザーの指定があれば、[M_HandyPageUser]テーブルを参照する
            string addLeftOuter = "";
            if (handyUserID > 0) addLeftOuter += $@" LEFT OUTER JOIN M_HandyPageUser AS B ON B.HandyPageID = A.HandyPageID ";

            string addWhere = "";
            if (handyUserID > 0) addWhere += $@" AND B.HandyUserID = @HandyUserID "; // ユーザーの指定
            if (administratorFlag == 0)  addWhere += $@" AND A.AdministratorOnlyFlag = @AdministratorOnlyFlag ";

            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var query = $@"
                                        SELECT
                                               A.SortNumber AS HandyPageNumber
                                              ,A.HandyPageID
                                              ,A.HandyPageName
                                        FROM M_HandyPage AS A
                                        {addLeftOuter}
                                        WHERE 1=1
                                            AND A.DepoID = @DepoID
                                            AND A.NotUseFlag = @NotUseFlag
                                            {addWhere}
                                        ORDER BY SortNumber
                                                ";
                    var param = new
                    {
                        DepoID = depoID,
                        NotUseFlag = 0,
                        HandyUserID = handyUserID,
                        AdministratorOnlyFlag = 0,
                    };
                    selectList = connection.Query<M_HandyPage>(query, param).ToList();

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
