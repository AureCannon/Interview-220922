using System.Threading.Tasks;

namespace Ct.Interview.Web.Api
{
    public interface IAsxListedCompaniesService
    {
        Task<AsxListedCompany[]> GetByAsxCode(string asxCode);
    }
}
