using Ct.Domain.Factories;

namespace Ct.Domain.Extensions
{
    public static class AsxHttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetAsync(this AsxHttpClient client, Uri uri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var responseMessage = await client.SendAsync(request);

            return responseMessage;
        }
    }
}
