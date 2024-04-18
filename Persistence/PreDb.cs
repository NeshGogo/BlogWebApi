using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class PreDb
    {
        public static void PrePopulation(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            SeedData(dbContext);
        }

        private static void SeedData(AppDbContext dbContext)
        {
            try
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations. Error: {ex.Message}");
            }
        }
    }
}
