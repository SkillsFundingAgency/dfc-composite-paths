using DFC.Common.Standard.Logging;
using DFC.Composite.Paths;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(StartUp))]
namespace DFC.Composite.Paths
{
    public class StartUp : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var services = builder.Services;
            RegisterServices(services);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            services.AddTransient<ILoggerHelper, LoggerHelper>();
            services.AddTransient<IHttpRequestHelper, HttpRequestHelper>();
        }
    }
}
