using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Chat.Entities.Contexts;

namespace Chat.Entities.ContextFactories
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();

            ConfigurationBuilder builder = new();

            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(
                connectionString: connectionString,
                sqlServerOptionsAction: options =>
                {
                    options.CommandTimeout((int)TimeSpan.FromSeconds(10).TotalSeconds);
                });

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
