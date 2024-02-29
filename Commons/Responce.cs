using Microsoft.AspNetCore.Mvc;

namespace SakaguraAGFWebApi.Commons
{
    public class Responce
    {

        public static IActionResult ExBadRequest(string message = "不正なリクエスト")
        {
            throw new HttpResponseException(StatusCodes.Status400BadRequest, message);
        }

        public static IActionResult ExNotFound(string message = "データが存在しません")
        {
            throw new HttpResponseException(StatusCodes.Status404NotFound, message);
        }

        public static IActionResult ExServerError(Exception exception, string message = "サーバーエラー")
        {
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, message, exception);
        }
    }
}
