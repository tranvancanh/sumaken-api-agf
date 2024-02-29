using Microsoft.AspNetCore.Mvc;
using SakaguraAGFWebApi.Commons;
using SakaguraAGFWebApi.Models;

namespace SakaguraAGFWebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TemporaryStoreAddressController : ControllerBase
    {
        [HttpGet("{companyID}")]
        public IActionResult Get(int companyID, int depoID)
        {
            var companys = CompanyModel.GetCompanyByCompanyID(companyID);
            if (companys.Count != 1) return Responce.ExNotFound("データベースの取得に失敗しました");
            var databaseName = companys[0].DatabaseName;

            var temporaryStoreAddresses = new List<TemporaryStoreAddressModel.M_TemporaryStoreAddress>();
            try
            {
                temporaryStoreAddresses = TemporaryStoreAddressModel.GetTemporaryStoreAddress(databaseName, depoID);
            }
            catch (Exception ex)
            {
                return Responce.ExServerError(ex);
            }

            return temporaryStoreAddresses.Count() == 0 ? Responce.ExNotFound("仮番地情報の取得に失敗しました") : Ok(temporaryStoreAddresses);
        }

    }
}
