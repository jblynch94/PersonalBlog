using PersonalBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace PersonalBlog.Helpers
{
    public class DataHelper
    {public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            await dbContextSvc.Database.MigrateAsync();
        }
    }
}
