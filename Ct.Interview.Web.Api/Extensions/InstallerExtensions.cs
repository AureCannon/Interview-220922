using Ct.Interview.Web.Api.Installers;

namespace Ct.Interview.Web.Api.Extensions
{
    public static class InstallerExtensions
    {
        public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
                typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.Configure(builder));

            return builder;
        }
    }
}
