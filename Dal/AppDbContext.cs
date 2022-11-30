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
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Kinopoisk_Onboarding;Username=root;Password=root");

            base.OnConfiguring(optionsBuilder);
        }
    }
}