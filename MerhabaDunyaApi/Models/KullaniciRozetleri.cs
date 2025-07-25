using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerhabaDunyaApi.Models
{
    public class KullaniciRozetleri
    {
        [Key]
        public int Id { get; set; }              // Tekil PK

        [ForeignKey(nameof(Kullanici))]
        public int KullaniciKimlik { get; set; } // FK → Kullanici.Kimlik

        [ForeignKey(nameof(Rozet))]
        public int RozetId { get; set; }         // FK → Rozet.Id

        public DateTime VerilisTarihi { get; set; }

        // Navigasyon
        public Kullanici Kullanici { get; set; } = null!;
        public Rozet Rozet { get; set; } = null!;
    }
}
