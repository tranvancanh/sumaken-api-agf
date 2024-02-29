using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class QrcodeController : ControllerBase
    {
        [HttpGet("{companyID}")]
        public IActionResult Get(int companyID, int depoID, int handyPageID)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var qrcodeIndices = new List<QrcodeModel.M_QrcodeIndex>();
            try
            {
                qrcodeIndices = QrcodeModel.GetQrcodeIndex(databaseName, depoID, handyPageID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            return qrcodeIndices.Count() == 0 ? Responce.ExNotFound("QRコードマスタが存在しません") : Ok(qrcodeIndices);
        }

    }
}
