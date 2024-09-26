using Microsoft.EntityFrameworkCore;

namespace CarScrapper.Entities
{
    public class ScrapperDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public ScrapperDbContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<CarScrapped> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                _config["ConnectionString"],
                options => options.EnableRetryOnFailure()
            );
        }

    }
}
