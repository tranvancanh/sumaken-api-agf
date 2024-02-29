using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;
//using Microsoft.AspNetCore.Http.HttpResults;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StoreInConstraintController : ControllerBase
    {
        [HttpGet("{companyID}")]
        public IActionResult Get(int companyID, int depoID)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var storeInConstraints = new List<StoreInConstraintModel.M_StoreInConstraint>();
            try
            {
                storeInConstraints = StoreInConstraintModel.GetStoreInConstraint(databaseName, depoID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            return storeInConstraints.Count() == 0 ? Responce.ExNotFound("在庫入庫制御情報の取得に失敗しました") : Ok(storeInConstraints);
        }

    }
}
