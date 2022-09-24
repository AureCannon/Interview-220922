using Ct.Domain.Models;

namespace Ct.Domain.Services
{
    public interface IAsxListedCompaniesService
    {
        Task<List<AsxListedCompany>> GetByAsxCodeAsync(string asxCode);
    }
}