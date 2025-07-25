using System.ComponentModel.DataAnnotations;

namespace MerhabaDunyaApi.DTOs
{
    public class KullaniciKayitDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string AdSoyad { get; set; }

        [Required]
        [EmailAddress]
        public string EPosta { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Sifre { get; set; }
    }
}