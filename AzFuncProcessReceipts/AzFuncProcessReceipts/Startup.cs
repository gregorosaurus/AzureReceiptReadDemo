using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzFuncProcessReceipts.Startup))]
namespace AzFuncProcessReceipts
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new Services.AzReceiptProceesor.Options()
            {
                FormRecognizerKey = Environment.GetEnvironmentVariable("DocIntelligenceKey"),
                FormRecoginzerEndpoint = Environment.GetEnvironmentVariable("DocIntelligenceEndpoint")
            });
            builder.Services.AddScoped<Services.IReceiptProcessor, Services.AzReceiptProceesor>();

            builder.Services.AddDbContext<Model.ReceiptsContext>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable("SqlServerConnectionString")!,
                    providerOptions => providerOptions.EnableRetryOnFailure());
            });
        }
    }
}
