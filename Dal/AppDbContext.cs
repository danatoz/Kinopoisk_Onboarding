using Microsoft.EntityFrameworkCore;
using Entities;

namespace Dal
{
    public class AppDbContext : DbContext
    {
        #region DbSet

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<MoviePremiereUpdateLog> MoviePremiereUpdateLogs { get; set; } = null!;

        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
}