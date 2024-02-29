using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HandyPageController : ControllerBase
    {
        [HttpGet("{companyID}")]
        public IActionResult Get(int companyID, int depoID, int administratorFlag, int handyUserID = 0)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var handyPages = new List<HandyPageModel.M_HandyPage>();
            try
            {
                handyPages = HandyPageModel.GetHandyPage(databaseName, depoID, administratorFlag, handyUserID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            return handyPages.Count() == 0 ? Responce.ExNotFound("表示可能なメニューがありません") : Ok(handyPages);
        }

    }
}
