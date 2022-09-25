using Ct.Domain.Services;
using Ct.Interview.Web.Api.Contracts;
using Ct.Interview.Web.Api.Mappers;
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

        [HttpGet("{asxCode}")]
        public async Task<ActionResult<AsxListedCompanyResponse[]>> Get(string asxCode)
        {
            var asxListedCompanies = await _asxListedCompaniesService.GetByAsxCodeAsync(asxCode);

            var response = asxListedCompanies.Select(AsxListedCompaniesMapper.ToDto);

            return base.Ok(response);
        }
    }
}
