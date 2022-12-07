using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;


namespace Dal.Concrete.Context
{
    public class AppDbContext : DbContext
    {
        #region DbSet

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;

        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}