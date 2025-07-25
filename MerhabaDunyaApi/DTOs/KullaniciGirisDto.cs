using System.ComponentModel.DataAnnotations;

namespace MerhabaDunyaApi.DTOs
{
    public class KullaniciGirisDto
    {
        [Required]
        [EmailAddress]
        public string EPosta { get; set; }

        [Required]
        public string Sifre { get; set; }
    }
}