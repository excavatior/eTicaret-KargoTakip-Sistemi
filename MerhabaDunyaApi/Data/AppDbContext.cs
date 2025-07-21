using MerhabaDunyaApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MerhabaDunyaApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<Kargo> Kargolar { get; set; }
        public DbSet<Rozet> Rozetler { get; set; }
        public DbSet<KullaniciRozetleri> KullaniciRozetleri { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<EmissionFactor> EmissionFactors { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<EmissionFactor>().HasKey(e => e.Kimlik);
        }
    }
}