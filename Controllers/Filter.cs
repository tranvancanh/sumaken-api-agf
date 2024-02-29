using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SakaguraAGFWebApi.Commons;

namespace SakaguraAGFWebApi.Controllers
{
    public class Filter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //var c = context.Controller as Controller;

            var requestMethod = context.HttpContext.Request.Method.ToString();
            var requestPath = context.HttpContext.Request.Path.ToString();
            var requestQueryString = context.HttpContext.Request.QueryString.ToString();
            var requestBody = "";

            // https://stackoverflow.com/questions/35589539/how-do-i-get-the-raw-request-body-from-the-request-content-object-using-net-4-a
            using (var streamReader = new StreamReader(context.HttpContext.Request.Body))
            {
                if (streamReader.BaseStream.Length > 0)
                {
                    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    requestBody = streamReader.ReadToEnd();
                }
            }

            var requestRouteString = "";
            var routeData = context.RouteData.Values;
            var request = new List<Dictionary<string, string>>();
            if (routeData.Count() > 0)
            {
                foreach (var data in routeData)
                {
                    var dic = new Dictionary<string, string>
                {
                    { data.Key, (string)(data.Value ?? "") }
                };
                    request.Add(dic);
                }
            }

            requestRouteString = JsonConvert.SerializeObject(routeData);

            var responseStatusCode = 0;

            var responseExceptionMessage = "";
            var responseInnerExceptionMessage = "";

            var responseBody = "";

            try
            {
                // https://www.kinakomotitti.net/entry/2021/02/14/141522
                if (context.Exception is HttpResponseException exception)
                {
                    context.Result = new ObjectResult(exception.Message)
                    {
                        StatusCode = exception.Status,
                    };
                    context.ExceptionHandled = true;
                    responseStatusCode = exception.Status;
                    responseExceptionMessage = (exception.Message ?? "").ToString();
                    if(exception.InnerException != null) responseInnerExceptionMessage = (exception.InnerException.Message ?? "").ToString();
                }
                else
                {
                    responseStatusCode = context.HttpContext.Response.StatusCode;

                    // Okeyの場合のレスポンスボディは、内容が大きいこともあるため
                    // 記録したい場合のみ、会社ごとのテーブル[D_HandyReportLog]に書き込むこととする！
                    // http://honeplus.blog50.fc2.com/blog-entry-210.html
                    //using (var streamReader = new StreamReader(context.HttpContext.Response.Body))
                    //{
                    //    if (streamReader.BaseStream.Length > 0)
                    //    {
                    //        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    //        responseBody = streamReader.ReadToEnd();
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {

            }

            var connectionString = new GetMasterConnectString().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {

                    var sqlQuery = @"
                                    INSERT D_API_RequestResponceLog
                                        (
                                        RequestMethod, 
                                        RequestPath,
                                        RequestQueryString,
                                        RequestRouteString,
                                        RequestBody,
                                        ResponseStatusCode, 
                                        ResponseBody,
                                        ResponseExceptionMessage, 
                                        ResponseInnerExceptionMessage, 
                                        CreateDate
                                        )
                                    VALUES
                                        (@RequestMethod, @RequestPath, @RequestQueryString, @RequestRouteString, @RequestBody, @ResponseStatusCode, @ResponseBody, @ResponseExceptionMessage, @ResponseInnerExceptionMessage, @CreateDate);
                                            ";
                    var param = new
                    {
                        RequestMethod = requestMethod,
                        RequestPath = requestPath,
                        RequestQueryString = requestQueryString,
                        RequestRouteString = requestRouteString,
                        RequestBody = requestBody,
                        ResponseStatusCode = responseStatusCode,
                        ResponseBody = responseBody,
                        ResponseExceptionMessage = responseExceptionMessage,
                        ResponseInnerExceptionMessage = responseInnerExceptionMessage,
                        CreateDate = DateTime.Now
                    };
                    var logInsert = connection.Execute(sqlQuery, param);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
    }
}
