using Ct.Domain.Models;
using Ct.Interview.Web.Api.Contracts;

namespace Ct.Interview.Web.Api.Mappers
{
    internal class AsxListedCompaniesMapper
    {
        public static AsxListedCompanyResponse ToDto(AsxListedCompany model)
        {
            return new AsxListedCompanyResponse
            {
                AsxCode = model.AsxCode,
                CompanyName = model.CompanyName
            };
        }
    }
}
