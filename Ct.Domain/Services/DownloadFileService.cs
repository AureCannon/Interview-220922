using Ct.Domain.Extensions;
using Ct.Domain.Factories;
using Ct.Domain.Models.Streams;
using Ct.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ct.Domain.Services
{
    public sealed class DownloadFileService : IDownloadFileService
    {
        private readonly AsxHttpClient _httpClient;
        private readonly AsxSettings _asxSettings;
        private readonly ILogger<DownloadFileService> _logger;

        public DownloadFileService(
            IAsxHttpClientFactory asxHttpClientFactory,
            IOptions<AsxSettings> asxSettings,
            ILogger<DownloadFileService> logger)
        {
            _httpClient = asxHttpClientFactory.CreateClient();
            _asxSettings = asxSettings.Value;
            _logger = logger;
        }

        public async Task<TemporaryFileStream> DownloadFileAsync()
        {
            var uri = new Uri(_asxSettings.ListedSecuritiesCsvUrl, UriKind.Absolute);

            _logger.LogInformation("Downloading csv file from ASX endpoint.");

            using var response = await _httpClient.GetAsync(uri);

            _logger.LogInformation("Finish downloading csv file from ASX endpoint.");

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await TemporaryFileStream.CreateFromStreamAsync(stream);
        }
    }
}
