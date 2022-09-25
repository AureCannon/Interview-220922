namespace Ct.Domain.Factories
{
    public sealed class AsxHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public AsxHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
