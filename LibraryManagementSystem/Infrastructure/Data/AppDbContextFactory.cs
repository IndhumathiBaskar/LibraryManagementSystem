using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LibraryManagementSystem.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {

            /*
            using Microsoft.Extensions.Configuration;: This namespace is needed for IConfiguration.
            using System.IO;: This namespace is needed for Directory.
            var configuration = new ConfigurationBuilder()...: This creates a new configuration builder.
            .SetBasePath(Directory.GetCurrentDirectory()): This sets the base path for finding appsettings.json to the current directory of your project.
            .AddJsonFile("appsettings.json"): This tells the builder to load configuration from appsettings.json.
            .Build(): This builds the IConfiguration object.
            var connectionString = configuration.GetConnectionString("DefaultConnection");: This retrieves the connection string from the configuration.
            optionsBuilder.UseSqlServer(connectionString);: This sets the connection string.
            */
            // Get the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Make sure the base path is the current directory
                .AddJsonFile("Presentation/appsettings.json") // Correct path to appsettings.json in Presentation folder
                .Build();

            // Get the connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Build the DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
