using System.ComponentModel.DataAnnotations;  // Birincil anahtar yapmamız için bunu kullanıyoruz

namespace MerhabaDunyaApi.Models
{
    public class Rozet
    {
        [Key]                                   // ← Birincil anahtar
        public int Id { get; set; }

        public string Ad { get; set; } = null!;
        public string Aciklama { get; set; } = null!;
        public string IconUrl { get; set; } = null!;
        public decimal RequiredSavePct { get; set; }
    }
}
