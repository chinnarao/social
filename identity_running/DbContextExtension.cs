using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using identity;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace identity_running
{
    public static class DbContextExtension
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeeded(this AppUsersDbContext context)
        {
            if (!context.UserRoles.Any())
            {
                var roles = JsonConvert.DeserializeObject<List<IdentityRole>>(File.ReadAllText("seed" + Path.DirectorySeparatorChar + "roles.json"));
                List<ApplicationRole> roleList = new List<ApplicationRole>();
                foreach (var role in roles)
                {
                    roleList.Add(new ApplicationRole() { Name = role.Name });
                }
                context.Roles.AddRange(roleList);
                int i = context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                List<ApplicationUser> appUserList = new List<ApplicationUser>();
                var userList = JsonConvert.DeserializeObject<List<ApplicationUser>>(File.ReadAllText("seed" + Path.DirectorySeparatorChar + "users.json"));
                foreach (var appUser in userList)
                {
                    string passwordHasher = new PasswordHasher<ApplicationUser>().HashPassword(appUser, appUser.PasswordHash);
                    appUser.PasswordHash = passwordHasher;
                    appUserList.Add(appUser);
                }
                context.Users.AddRange(appUserList);
                int j = context.SaveChanges();
            }
        }
    }
}
