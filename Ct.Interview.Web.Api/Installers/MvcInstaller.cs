using Ct.Domain.Factories;

namespace Ct.Interview.Web.Api.Installers
{
    public sealed class MvcInstaller : IInstaller
    {
        public void Configure(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            AddHttpClient(services);

            services.AddControllers();
            services.AddEndpointsApiExplorer();
        }

        private static void AddHttpClient(IServiceCollection services)
        {
            _ = services.AddHttpClient(AsxHttpClientFactory.AsxHttpClient)
                        .ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new HttpClientHandler
                            {
                                AllowAutoRedirect = true,
                            };
                        });
        }
    }
}
