using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Ct.Domain.Factories;

namespace Ct.Web.Api.Tests
{
    public sealed class TestFixture : IDisposable
    {
        public TestFixture()
        {
            var application = new ASXListedCompaniesApplication();

            Client = application.CreateClient();
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
        }
    }

    class ASXListedCompaniesApplication : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddHttpClient(AsxHttpClientFactory.AsxHttpClient)
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new MockHttpClientHandler();
                        });
            });
        }
    }
}
