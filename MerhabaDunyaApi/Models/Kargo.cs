using System;
using System.ComponentModel.DataAnnotations;  // ← bu eklenmeli
using MerhabaDunyaApi.Models;  // eğer başka modelleri kullandığınız başka using’ler varsa

namespace MerhabaDunyaApi.Models
{
    public class Kargo
    {
        [Key]                                     // ← Kimlik’i PK olarak işaretliyoruz
        public int Kimlik { get; set; }
        public int SiparisKimlik { get; set; }
        public Siparis Siparis { get; set; } = null!;
        public string TakipNumarasi { get; set; } = null!;
        public DateTime? GonderimTarihi { get; set; }
        public string? Durum { get; set; }
    }
}