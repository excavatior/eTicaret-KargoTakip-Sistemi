using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // ← ekle

namespace MerhabaDunyaApi.Models
{
    public class Kullanici
    {
        [Key]                                   // ← Kimlik’i PK yapıyoruz
        public int Kimlik { get; set; }
            public string AdSoyad { get; set; }
            public string EPosta { get; set; }
            public string SifreHash { get; set; }
            public string SifreSalt { get; set; }
            public DateTime OlusturmaTarihi { get; set; }
            public DateTime? SonGirisTarihi { get; set; }
            public bool Aktif { get; set; }
        }
    }

