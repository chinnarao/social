using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identity
{
    public class AppUsersDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppUsersDbContext(DbContextOptions<AppUsersDbContext> options): base(options)
        {
        }

        public static AppUsersDbContext Create(string connection = "Server=localhost;Database=identity;Trusted_Connection=True;")
        {
            return new AppUsersDbContext(new DbContextOptionsBuilder<AppUsersDbContext>().UseSqlServer(connection).Options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
