using Ct.Domain.Models.Streams;

namespace Ct.Domain.Services
{
    public interface IDownloadFileService
    {
        Task<TemporaryFileStream> DownloadFileAsync();
    }
}