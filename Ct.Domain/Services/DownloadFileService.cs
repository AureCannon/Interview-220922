using System.Net;
using Ct.Domain.Exceptions;
using Ct.Domain.Extensions;
using Ct.Domain.Factories;
using Ct.Domain.Models.Streams;
using Ct.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace Ct.Domain.Services
{
    public sealed class DownloadFileService : IDownloadFileService
    {
        const int MaxRetries = 3;

        private readonly IAsxHttpClientFactory _httpClientFactory;
        private readonly AsxSettings _asxSettings;
        private readonly ILogger<DownloadFileService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _resilientPolicy;
        private readonly Random _randomDelay = new();

        public DownloadFileService(
            IAsxHttpClientFactory asxHttpClientFactory,
            IOptions<AsxSettings> asxSettings,
            ILogger<DownloadFileService> logger)
        {
            _httpClientFactory = asxHttpClientFactory;
            _asxSettings = asxSettings.Value;
            _logger = logger;
            _retryPolicy = CreateRetryPolicy();
            _circuitBreakerPolicy = CreateCircuitBreakerPolicy();
            _resilientPolicy = _circuitBreakerPolicy.WrapAsync(_retryPolicy);
        }

        public async Task<TemporaryFileStream> DownloadFileAsync()
        {
            if (_circuitBreakerPolicy.CircuitState == CircuitState.Open)
                throw new ServiceUnavailableException("Service is unavailable");

            var uri = new Uri(_asxSettings.ListedSecuritiesCsvUrl, UriKind.Absolute);

            var httpClient = _httpClientFactory.CreateClient();

            using var response = await _resilientPolicy.ExecuteAsync(async () => await httpClient.GetAsync(uri));

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await TemporaryFileStream.CreateFromStreamAsync(stream);
        }

        private AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
        {
            return Policy.HandleResult<HttpResponseMessage>(x => x.StatusCode == HttpStatusCode.ServiceUnavailable)
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
        }

        private AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy.Handle<HttpRequestException>()
                         .WaitAndRetryAsync(MaxRetries,
                                            retryCount => RetryDelay(retryCount),
                                            (_, timeSpan, retryCount, _) => OnRetry(timeSpan, retryCount));
        }

        private void OnRetry(TimeSpan timeSpan, int retryCount) =>
            _logger.LogWarning($"Service delivery attempt {retryCount} failed, next attempt in {timeSpan.TotalMilliseconds} ms.");

        private TimeSpan RetryDelay(int retryCount)
        {
            var retrySquaredTime = TimeSpan.FromSeconds(Math.Pow(2, retryCount));

            var randomTimeDelay = TimeSpan.FromMilliseconds(_randomDelay.Next(0, 1000));

            return retrySquaredTime + randomTimeDelay;
        }
    }
}
