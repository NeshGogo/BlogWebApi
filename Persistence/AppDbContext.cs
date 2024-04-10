using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class AppDbContext : IdentityDbContext
    {
        public const string Schema = "BlogPost";
        public AppDbContext(DbContextOptions opt) : base(opt)
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
