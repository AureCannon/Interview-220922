namespace Ct.Domain.Factories
{
    public sealed class AsxHttpClientFactory : IAsxHttpClientFactory
    {
        public const string AsxHttpClient = "AsxHttpClient";

        private readonly IHttpClientFactory _httpClientFactory;

        public AsxHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public AsxHttpClient CreateClient()
        {
            var httpClient = _httpClientFactory.CreateClient(AsxHttpClient);

            return new AsxHttpClient(httpClient);
        }
    }
}
