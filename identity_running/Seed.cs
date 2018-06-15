using identity;
using Microsoft.EntityFrameworkCore;

namespace identity_running
{
    public class Seed
    {
        public Seed(){
            var appUserContext = AppUsersDbContext.Create();
            if (appUserContext.AllMigrationsApplied())
            {
                appUserContext.Database.Migrate();
                appUserContext.EnsureSeeded();
            }
        }
    }
}
