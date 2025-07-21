using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // ← Ekledik

namespace MerhabaDunyaApi.Models
{
    public class Siparis
    {
        [Key]                                   // ← Birincil anahtar
        public int Kimlik { get; set; }

        public int KullaniciKimlik { get; set; }
        public Kullanici Kullanici { get; set; } = null!;

        public DateTime SiparisTarihi { get; set; }
        public string Durum { get; set; } = null!;
        public decimal ToplamTutar { get; set; }

        public ICollection<Kargo> Kargolar { get; set; } = new List<Kargo>();
    }
}
