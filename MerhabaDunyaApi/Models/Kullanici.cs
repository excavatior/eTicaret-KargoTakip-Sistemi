using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // ← ekle

namespace MerhabaDunyaApi.Models
{
    public class Kullanici
    {
        [Key]                                   // ← Kimlik’i PK yapıyoruz
        public int Kimlik { get; set; }
        public string AdSoyad { get; set; } = null!;
        public string EPosta { get; set; } = null!;
        public string SifreHash { get; set; } = null!;
        public DateTime OlusturmaTarihi { get; set; }

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
    }
}
