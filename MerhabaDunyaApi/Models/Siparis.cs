using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerhabaDunyaApi.Models
{
    public class Siparis
    {
        [Key]
        public int Kimlik { get; set; }          // PK

        [ForeignKey(nameof(Kullanici))]
        public int KullaniciKimlik { get; set; } // FK → Kullanici.Kimlik
        public Kullanici Kullanici { get; set; } = null!;

        public DateTime SiparisTarihi { get; set; }
        public string Durum { get; set; } = null!;
        public decimal ToplamTutar { get; set; }

        // 1–1 ilişki
        public Kargo? Kargo { get; set; }
    }
}
