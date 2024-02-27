using Microsoft.EntityFrameworkCore;
using SecurePassword.Models.DbReturns;

namespace SecurePassword.Context
{
    public class MyContext : DbContext
    {
        IConfiguration _configuration;
        public MyContext(DbContextOptions<MyContext> options, IConfiguration Configuration) : base(options)
        {
            _configuration = Configuration;
        }
        //"Server=localhost\\SQLEXPRESS;Database=Lagerstyringssystem;Trusted_Connection=true;TrustServerCertificate=true;"
        //"Server=localhost\\MSSQLSERVER03;Database=Lagerstyringssystem;Trusted_Connection=true;TrustServerCertificate=true;"
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("YourConnectionString"));

        public DbSet<LoginDbReturn> LoginDbReturn { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginDbReturn>().HasNoKey();
        }
    }
}

