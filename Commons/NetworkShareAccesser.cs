using Dapper;
using SakaguraAGFWebApi.Commons;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace sumaken_api_agf.Commons
{
    public class NetworkShareAccesser : IDisposable
    {

        string _remoteUncName;
        string _userName;
        string _password;
        bool _connected = false;

        public bool Connected => _connected;

        public NetworkShareAccesser(string remoteUncName, NetworkCredential credentials)
        {
            _remoteUncName = remoteUncName;
            _userName = credentials.UserName;
            _password = credentials.Password;

            var value = ConnectToRemote(_remoteUncName, _userName, _password);
            if(value == 0 || value == 1219)
                _connected = true;
            else
                _connected = false;
        }

        public void Dispose()
        {
            if (_connected)
                DisconnectRemote(_remoteUncName);
        }

        int ConnectToRemote(string remoteUnc, string username, string password)
        {
            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = remoteUnc
            };

            var result = WNetAddConnection2(
                netResource,
                password,
                username,
                0);

            return result;
        }

        void DisconnectRemote(string remoteUnc)
        {
            WNetCancelConnection2(remoteUnc, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);


        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }

        public enum Level : int
        {
            Infor = 0,
            Error = 1
        }

        /////////////////////////////////////////////////////////////////////////////////////////

        public static async Task<(Level Level, string Mess)> CheckAccessServerOrSharedResource(string databaseName, string companyCode)
        {
            (Level level, string mess) result = new (Level.Infor, "");
            var agf_shared_folder = string.Empty;
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT 
                                [CompanyCode],
                                [AGFApiUrl],
                                [agf_shared_folders]
                            FROM [M_AGF_WebAPIURL]
                            WHERE [CompanyCode] = @CompanyCode
                            ";
                var param = new
                {
                    CompanyCode = companyCode
                };
                var table = new DataTable();
                var reader = await connection.ExecuteReaderAsync(query, param);
                table.Load(reader);
                if (table.Rows.Count <= 0)
                {
                    throw new Exception("CSVの落とし先共有フォルダが存在していません");
                }
                agf_shared_folder = Convert.ToString(table.Rows[0]["agf_shared_folders"]);
            }

            // Remote folder path
            var remoteFolderPath = agf_shared_folder;

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false);
            var configurationRoot = builder.Build();
            // Username and password for accessing the remote folder
            var configuration = configurationRoot.GetSection("agfSharedFolders");
            var userName = configuration.GetValue<string>("userName");
            var passWord = configuration.GetValue<string>("passWord");

            // Create a NetworkCredential object with the specified userName and passWord
            NetworkCredential credentials = new NetworkCredential(userName, passWord);

            // Ensure the directory exists or create it if it doesn't
            if (!Directory.Exists(remoteFolderPath))
            {
                // Access the remote folder using the NetworkShareAccesser class to validate credentials
                using (var accesser = new NetworkShareAccesser(remoteFolderPath, credentials))
                {
                    // If the connection was successful, proceed to create the directory
                    if (accesser.Connected)
                    {
                        // Ensure the directory exists or create it if it doesn't
                        if (!Directory.Exists(remoteFolderPath))
                        {
                            try
                            {
                                Directory.CreateDirectory(remoteFolderPath);
                                Debug.WriteLine("Directory created successfully.");
                                result = new (Level.Infor, "$Directory created successfully.");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error creating directory: {ex.Message}");
                                result = new(Level.Error, "$Error creating directory: {ex.Message}");
                                throw;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Directory already exists.");
                            result = new(Level.Infor, "$Directory already exists.");
                        }

                        // Now you can perform operations on the remote folder
                        // For example, you can list the files in the folder
                        string[] files = Directory.GetFiles(remoteFolderPath);
                        foreach (string file in files)
                        {
                            Debug.WriteLine(file);
                        }
                    }
                    if (!Directory.Exists(remoteFolderPath))
                    {
                        Directory.CreateDirectory(remoteFolderPath);
                        Debug.WriteLine("Directory created successfully.");
                        result = new(Level.Infor, "Directory created successfully.");
                    }
                }
            }
            else
            {
                Debug.WriteLine("Directory already exists.");
                result = new(Level.Infor, "Directory already exists.");
            }

            return result;
        }
    }
}
