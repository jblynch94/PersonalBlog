using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PersonalBlog.Models;

namespace PersonalBlog.Data
{
    public static class DataUtility
    {
        private const string _adminEmail = "jacobbrocklynch@gmail.com";
        private const string _modEmail = "jacobbrocklynch@gmail.com";
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";

        public static DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task SeedDataAsync(IServiceProvider svcProvider)
        {
            //Service: am instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Migration: this is the programmatic equivelent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            //service: an instance of Configuration Service
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
            //service: an instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //service: an instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            //Seed roles
            await SeedRolesAsync(roleManagerSvc);
            //Seed Users
            await SeedUsersAsync(dbContextSvc,configurationSvc,userManagerSvc);
        }
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }
        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            //add Admin
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Jacob",
                    LastName = "Lynch",
                    PhoneNumber = "1234567890",
                    EmailConfirmed = true

                };
                await userManager.CreateAsync(adminUser, configuration["AdminPwd"]);
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }

            //add Moderator

            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "Brock",
                    LastName = "Lynch",
                    PhoneNumber = "1234567890",
                    EmailConfirmed = true

                };
                await userManager.CreateAsync(modUser, configuration["ModeratorPwd"]);
                await userManager.AddToRoleAsync(modUser, _modRole);
            }

        }
    }
}
