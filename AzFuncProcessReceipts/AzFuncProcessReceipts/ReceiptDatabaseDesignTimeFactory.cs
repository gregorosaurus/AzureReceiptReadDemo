using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AzFuncProcessReceipts
{

    public class ReceiptDatabaseDesignTimeFactory : IDesignTimeDbContextFactory<Model.ReceiptsContext>
    {
        public Model.ReceiptsContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: false)
                .Build();

            string connString = configuration.GetSection("Values").GetValue<string>("SqlServerConnectionString")!;
            Console.WriteLine($"Running with connection string {connString}");

            var optionsBuilder = new DbContextOptionsBuilder<Model.ReceiptsContext>();
            optionsBuilder.UseSqlServer(connString);

            return new Model.ReceiptsContext(optionsBuilder.Options);
        }
    }
}
