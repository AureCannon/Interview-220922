using Microsoft.AspNetCore.Mvc;

namespace Ct.Interview.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsxListedCompaniesController : ControllerBase
    {
        private readonly IAsxListedCompaniesService _asxListedCompaniesService;

        public AsxListedCompaniesController(IAsxListedCompaniesService asxListedCompaniesService)
        {
            _asxListedCompaniesService = asxListedCompaniesService;
        }

        [HttpGet]
        public async Task<ActionResult<AsxListedCompanyResponse[]>> Get(string asxCode)
        {
            var asxListedCompanies = await _asxListedCompaniesService.GetByAsxCode(asxCode);

            return Ok(asxListedCompanies);
        }
    }
}
