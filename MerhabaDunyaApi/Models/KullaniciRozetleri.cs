using System;
using System.ComponentModel.DataAnnotations;

namespace MerhabaDunyaApi.Models
{
    public class KullaniciRozetleri
    {
        [Key]
        public int Id { get; set; }  // Yeni tekil PK

        public int KullaniciKimlik { get; set; }
        public int RozetId { get; set; }
        public DateTime VerilisTarihi { get; set; }

        public Rozet Rozet { get; set; } = null!;
        public Kullanici Kullanici { get; set; } = null!;
    }
}
