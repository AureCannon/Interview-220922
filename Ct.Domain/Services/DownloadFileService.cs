using Ct.Domain.Extensions;
using Ct.Domain.Factories;
using Ct.Domain.Models.Streams;
using Ct.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Ct.Domain.Services
{
    public sealed class DownloadFileService : IDownloadFileService
    {
        const int MaxRetries = 3;

        private readonly AsxHttpClient _httpClient;
        private readonly AsxSettings _asxSettings;
        private readonly ILogger<DownloadFileService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public DownloadFileService(
            IAsxHttpClientFactory asxHttpClientFactory,
            IOptions<AsxSettings> asxSettings,
            ILogger<DownloadFileService> logger)
        {
            _httpClient = asxHttpClientFactory.CreateClient();
            _asxSettings = asxSettings.Value;
            _logger = logger;
            _retryPolicy = Policy.Handle<HttpRequestException>(x =>
                            {                                    
                                _logger.LogError(x.Message);
                                return true;
                            })
                            .WaitAndRetryAsync(MaxRetries, x => TimeSpan.FromMilliseconds(x * 100));
        }

        public async Task<TemporaryFileStream> DownloadFileAsync()
        {
            var uri = new Uri(_asxSettings.ListedSecuritiesCsvUrl, UriKind.Absolute);

            return await _retryPolicy.ExecuteAsync(() => GetFileAsync(uri));
        }

        private async Task<TemporaryFileStream> GetFileAsync(Uri uri)
        {         
            using var response = await _httpClient.GetAsync(uri);

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await TemporaryFileStream.CreateFromStreamAsync(stream);
        }
    }
}
