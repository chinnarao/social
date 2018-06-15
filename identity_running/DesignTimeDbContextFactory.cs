using identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace identity_running
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppUsersDbContext>
    {
        public AppUsersDbContext CreateDbContext(string[] args) //you can ignore args, maybe on later versions of .net core it will be used but right now it isn't
        {
            IConfigurationRoot configuration = GetConfiguration();
            string connection = configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<AppUsersDbContext>().UseSqlServer(connection, b => b.MigrationsAssembly("identity_running")).Options;
            return new AppUsersDbContext(options);
        }

        private IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }
    }
}
