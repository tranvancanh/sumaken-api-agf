﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using sumaken_api_agf.Commons;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using technoleight_THandy.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sumaken_api_agf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgfCommonsController : ControllerBase
    {
        private readonly ILogger<AgfCommonsController> _logger;
        public AgfCommonsController(ILogger<AgfCommonsController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Nlog is started to AGF共通処理開始");
        }

        // GET: api/<AgfCommonsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AgfCommonsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AgfCommonsController>
        [HttpPost()]
        [Route("ScanRecord/{companyID}")]
        public async Task<IActionResult> ScanRecord(int companyID, [FromBody] List<AGFScanRecordModel> scanRecords)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;
            await SaveScanRecord(databaseName, scanRecords);
            return Ok();
        }

        private async Task<int> SaveScanRecord(string databaseName, List<AGFScanRecordModel> scanRecords)
        {
            var affectedRows = 0;
            var connectionString = new GetConnectString(databaseName).ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        foreach(var item in scanRecords)
                        {
                            var query = @"
                            INSERT INTO [D_AGF_ScanRecord] 
                            ([DepoID],[HandyUserID],[HandyOperationClass],[HandyOperationMessage],[Device],[HandyPageID],[ScanString1],[ScanString2],[ScanString3],[ScanTime],[Latitude],[Longitude],[CreateDate])
                            VALUES (@DepoID,@HandyUserID,@HandyOperationClass,@HandyOperationMessage,@Device,@HandyPageID,@ScanString1,@ScanString2,@ScanString3,@ScanTime,@Latitude,@Longitude,@CreateDate)
                            ;";
                            var param = new
                            {
                                DepoID = item.DepoID,
                                HandyUserID = item.HandyUserID,
                                HandyOperationClass = item.HandyOperationClass,
                                HandyOperationMessage  = item.HandyOperationMessage,
                                Device = item.Device,
                                HandyPageID = item.HandyPageID,
                                ScanString1 = item.ScanString1,
                                ScanString2 = item.ScanString2,
                                ScanString3 = item.ScanString3,
                                ScanTime = item.ScanTime,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude,
                                CreateDate = DateTime.Now
                            };
                            var result = await connection.ExecuteAsync(query, param, tran);
                            if(result > 0) { affectedRows =  affectedRows + result; }
                        }

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            return affectedRows;
        }

        [HttpGet()]
        [Route("CheckSaveCSVPath/{companyID}")]
        public async Task<IActionResult> CheckSaveCSVPath(int companyID)
        {
            try
            {
                var companys = CompanyModel.GetCompanyByCompanyID(companyID);
                if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
                var databaseName = companys[0].DatabaseName;
                var companyCode = companys[0].CompanyCode;

                var result = await CheckAccessServerOrSharedResource(databaseName, companyCode);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("AGF共有フォルダはエラーが発生しました。");
                _logger.LogError("Message   ：   " + ex.Message);
                return StatusCode(500, ex.Message);
            }
            
        }

        public static async Task<bool> CheckAccessServerOrSharedResource(string databaseName, string companyCode)
        {
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
                                Console.WriteLine("Directory created successfully.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error creating directory: {ex.Message}");
                                throw;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Directory already exists.");
                        }

                        // Now you can perform operations on the remote folder
                        // For example, you can list the files in the folder
                        string[] files = Directory.GetFiles(remoteFolderPath);
                        foreach (string file in files)
                        {
                            Console.WriteLine(file);
                        }
                    }
                    if (!Directory.Exists(remoteFolderPath))
                    {
                        Directory.CreateDirectory(remoteFolderPath);
                        Console.WriteLine("Directory created successfully.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Directory already exists.");
            }

            return true;
        }
    }
}
