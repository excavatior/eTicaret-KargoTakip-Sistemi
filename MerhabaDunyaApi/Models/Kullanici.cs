using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MerhabaDunyaApi.Models
{
    public class Kullanici
    {
        [Key]
        public int Kimlik { get; set; }

        [Required, MaxLength(100)]
        public string AdSoyad { get; set; } = string.Empty;

        [Required, MaxLength(100), EmailAddress]
        public string EPosta { get; set; } = string.Empty;

        [Required]
        public string SifreHash { get; set; } = string.Empty;

        // ← MUTLAKA byte[] olacak, EF Core varbinary(max) ile eşleşir
        [Required]
        public byte[] SifreSalt { get; set; } = Array.Empty<byte>();

        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public bool Aktif { get; set; }

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public ICollection<KullaniciRozetleri> KullaniciRozetleri { get; set; } = new List<KullaniciRozetleri>();
    }
}
