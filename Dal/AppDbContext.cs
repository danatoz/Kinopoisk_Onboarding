using Microsoft.EntityFrameworkCore;
using Entities;

namespace Dal
{
    public class AppDbContext : DbContext
    {
        #region DbSet
        private DbSet<Country> Countries { get; set; } = null!;

        #endregion
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}