using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerhabaDunyaApi.Models
{
    public class Kargo
    {
        [Key]
        public int Kimlik { get; set; }           // PK

        [ForeignKey(nameof(Siparis))]
        public int SiparisKimlik { get; set; }    // FK → Siparis.Kimlik
        public Siparis Siparis { get; set; } = null!;

        public string TakipNumarasi { get; set; } = null!;
        public DateTime? GonderimTarihi { get; set; }
        public string? Durum { get; set; }
    }
}
