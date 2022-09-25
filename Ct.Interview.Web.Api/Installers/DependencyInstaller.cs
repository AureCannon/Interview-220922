using Ct.Domain;
using Ct.Domain.Settings;

namespace Ct.Interview.Web.Api.Installers
{
    public sealed class DependencyInstaller : IInstaller
    {
        public void Configure(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            DomainDependency.Configure(services);

            services.Configure<AsxSettings>(builder.Configuration.GetSection(nameof(AsxSettings)));
        }
    }
}
