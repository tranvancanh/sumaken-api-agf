using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
using static SakaguraAGFWebApi.Models.HandyAdminCheckModel;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HandyAdminCheckController : ControllerBase
    {
        // POST: api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody] HandyAdminCheckPostBody input)
        {
            var handyAdminCount = 0;
            try
            {
                handyAdminCount = HandyAdminCheckModel.HandyAdminCheck(input.CompanyID, input.HandyAdminPassword);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }
            if (handyAdminCount != 1) return Responce.ExBadRequest("管理者パスワードが不正です");

            return Ok();

        }

      }
}
