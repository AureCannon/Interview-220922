using Ct.Domain.Exceptions;
using Ct.Domain.Models;
using Ct.Domain.Parsers;

namespace Ct.Domain.Services
{
    public class AsxListedCompaniesService : IAsxListedCompaniesService
    {
        private readonly IDownloadFileService _downloadFileService;
        private readonly ICsvFileStreamParser _csvFileStreamParser;

        public AsxListedCompaniesService(
            IDownloadFileService downloadFileService,
            ICsvFileStreamParser csvFileStreamParser)
        {
            _downloadFileService = downloadFileService;
            _csvFileStreamParser = csvFileStreamParser;
        }

        public async Task<List<AsxListedCompany>> GetByAsxCodeAsync(string asxCode)
        {
            var tempFileStream = await _downloadFileService.DownloadFileAsync();

            var asxCompanies = _csvFileStreamParser.Parse(tempFileStream);

            if (asxCompanies == null)
                return new List<AsxListedCompany>();

            if (asxCompanies.TryGetValue(asxCode, out var companies))
                return companies;

            throw new RecordNotFoundException("ASX code does not exist.");
        }
    }
}
