using Ct.Domain.Exceptions;
using Ct.Domain.Extensions;
using Ct.Domain.Factories;
using Ct.Domain.Models.Streams;
using Ct.Domain.Policies;
using Ct.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ct.Domain.Services
{
    public sealed class DownloadFileService : IDownloadFileService
    {
        private readonly IAsxHttpClientFactory _httpClientFactory;
        private readonly AsxSettings _asxSettings;
        private readonly ILogger<DownloadFileService> _logger;
        private readonly IResilientPolicy _resilientPolicy;

        public DownloadFileService(
            IAsxHttpClientFactory asxHttpClientFactory,
            IOptions<AsxSettings> asxSettings,
            ILogger<DownloadFileService> logger,
            IResilientPolicy resilientPolicy)
        {
            _httpClientFactory = asxHttpClientFactory;
            _asxSettings = asxSettings.Value;
            _logger = logger;
            _resilientPolicy = resilientPolicy;
        }

        public async Task<TemporaryFileStream> DownloadFileAsync()
        {
            if (_resilientPolicy.IsCircuitOpen)
                throw new ServiceUnavailableException("Service is unavailable");

            var uri = new Uri(_asxSettings.ListedSecuritiesCsvUrl, UriKind.Absolute);

            var httpClient = _httpClientFactory.CreateClient();

            using var response = await _resilientPolicy.ExecuteAsync(async () => await httpClient.GetAsync(uri));

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await TemporaryFileStream.CreateFromStreamAsync(stream);
        }
    }
}
