using MerhabaDunyaApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MerhabaDunyaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<EmissionFactor> EmissionFactors { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<EmissionFactor>().HasKey(e => e.Kimlik);
        }
    }
}