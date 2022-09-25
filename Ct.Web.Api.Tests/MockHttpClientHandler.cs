using System.Net;
using System.Text;

namespace Ct.Web.Api.Tests
{
    public class MockHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateCsvRespone());
        }

        private HttpResponseMessage CreateCsvRespone()
        {
            string csv = @"
ASX listed companies as at Thu Sep 22 19:48:59 AEST 2022

Company name,ASX code,GICS industry group
""MOQ LIMITED"",""MOQX"",""Software & Services""
""1414 DEGREES LIMITED"",""14D"",""Capital Goods""
""1ST GROUP LIMITED"",""1ST"",""Health Care Equipment & Services""
";
            var response = new HttpResponseMessage();
            response.Content = new StringContent(csv, Encoding.UTF8, "text/csv");
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}
