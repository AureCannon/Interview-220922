using Microsoft.OpenApi.Models;

namespace Ct.Interview.Web.Api.Installers
{
    public sealed class SwaggerInstaller : IInstaller
    {
        public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ASX Listed Companies",
                    Version = "v1"
                });
            });
        }
    }
}
