using Ct.Domain.Factories;
using Ct.Domain.Parsers;
using Ct.Domain.Policies;
using Ct.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ct.Domain
{
    public sealed class DomainDependency
    {
        public static void Configure(IServiceCollection services)
        {  
            services.AddScoped<IAsxHttpClientFactory, AsxHttpClientFactory>();
            services.AddScoped<IAsxListedCompaniesService, AsxListedCompaniesService>();
            services.AddScoped<IDownloadFileService, DownloadFileService>();
            services.AddScoped<ICsvFileStreamParser, CsvFileStreamParser>();            
            services.AddScoped<IResilientPolicy, ResilientPolicy>();
        }
    }
}
