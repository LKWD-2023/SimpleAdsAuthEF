using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class SimpleAdsAuthDataContext : DbContext
    {
        private string _connectionString;

        public SimpleAdsAuthDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SimpleAd> SimpleAds { get; set; }
    }

    public class SimpleAdsAuthDataContextFactory : IDesignTimeDbContextFactory<SimpleAdsAuthDataContext>
    {
        public SimpleAdsAuthDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}SimpleAdsAuth.Web"))
               .AddJsonFile("appsettings.json")
               .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new SimpleAdsAuthDataContext(config.GetConnectionString("ConStr"));
        }
    }
}
