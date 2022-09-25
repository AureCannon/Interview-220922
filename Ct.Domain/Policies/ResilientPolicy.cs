using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace Ct.Domain.Policies
{
    public class ResilientPolicy : IResilientPolicy
    {
        private const int MaxRetries = 3;

        private readonly ILogger<ResilientPolicy> _logger;
        private readonly Random _randomDelay = new();
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly AsyncPolicyWrap<HttpResponseMessage> _resilientPolicy;

        public ResilientPolicy(ILogger<ResilientPolicy> logger)
        {
            _logger = logger;
            _circuitBreakerPolicy = CreateCircuitBreakerPolicy();
            _resilientPolicy = WrapPolicies();
        }

        public Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> action)
        {
            return _resilientPolicy.ExecuteAsync(action);
        }

        public bool IsCircuitOpen => _circuitBreakerPolicy.CircuitState == CircuitState.Open;

        private AsyncPolicyWrap<HttpResponseMessage> WrapPolicies()
        {
            var retryPolicy = CreateRetryPolicy();

            return _circuitBreakerPolicy.WrapAsync(retryPolicy);
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
                                        retryCount => GetRetryDelay(retryCount),
                                        (_, timeSpan, retryCount, _) => OnRetry(timeSpan, retryCount));            
        }

        private void OnRetry(TimeSpan timeSpan, int retryCount) =>
            _logger.LogWarning($"Service delivery attempt {retryCount} failed, next attempt in {timeSpan.TotalMilliseconds} ms.");

        private TimeSpan GetRetryDelay(int retryCount)
        {
            var retryCountSquared = TimeSpan.FromSeconds(Math.Pow(2, retryCount));

            var randomTimeDelay = TimeSpan.FromMilliseconds(_randomDelay.Next(0, 1000));

            return retryCountSquared + randomTimeDelay;
        }
    }
}
