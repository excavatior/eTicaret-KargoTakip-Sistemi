using MerhabaDunyaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MerhabaDunyaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<Kargo> Kargolar { get; set; }
        public DbSet<Rozet> Rozetler { get; set; }
        public DbSet<KullaniciRozetleri> KullaniciRozetleri { get; set; }
        public DbSet<EmissionFactor> EmissionFactors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Kullanici>()
       .Property(u => u.SifreSalt)
       .HasColumnType("varbinary(max)");

            modelBuilder.Entity<EmissionFactor>()
       .ToTable("EmisyonFaktorleri")
       .HasKey(e => e.Kimlik);

            modelBuilder.Entity<EmissionFactor>()
                .Property(e => e.KgCO2PerLitre)
                .HasColumnType("decimal(18,2)");

            // Rozet.RequiredSavePct için
            modelBuilder.Entity<Rozet>()
                .Property(r => r.RequiredSavePct)
                .HasColumnType("decimal(18,2)");

            // Siparis.ToplamTutar için
            modelBuilder.Entity<Siparis>()
                .Property(s => s.ToplamTutar)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<KullaniciRozetleri>()
                .HasKey(kr => kr.Id);

            modelBuilder.Entity<KullaniciRozetleri>()
                .HasOne(kr => kr.Kullanici)
                .WithMany(k => k.KullaniciRozetleri)
                .HasForeignKey(kr => kr.KullaniciKimlik);

            modelBuilder.Entity<KullaniciRozetleri>()
                .HasOne(kr => kr.Rozet)
                .WithMany(r => r.KullaniciRozetleri)
                .HasForeignKey(kr => kr.RozetId);

            // --- Siparis ↔ Kullanici (1–N) ---
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Siparisler)
                .HasForeignKey(s => s.KullaniciKimlik);

            // --- Siparis ↔ Kargo (1–1) ---
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kargo)
                .WithOne(k => k.Siparis)
                .HasForeignKey<Kargo>(k => k.SiparisKimlik);

            // --- EmissionFactor PK ---
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
