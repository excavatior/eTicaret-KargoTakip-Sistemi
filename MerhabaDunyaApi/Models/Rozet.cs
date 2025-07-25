using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MerhabaDunyaApi.Models
{
    public class Rozet
    {
        [Key]                                   // Birincil anahtar
        public int Id { get; set; }

        public string Ad { get; set; } = null!;
        public string Aciklama { get; set; } = null!;
        public string IconUrl { get; set; } = null!;
        public decimal RequiredSavePct { get; set; }

        // Navigasyon özelliği: Kullanıcılarla ilişki (join tablosu)
        public ICollection<KullaniciRozetleri> KullaniciRozetleri { get; set; } = new List<KullaniciRozetleri>();
    }
}