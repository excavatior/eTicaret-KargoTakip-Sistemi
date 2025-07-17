using System.ComponentModel.DataAnnotations;
namespace MerhabaDunyaApi.Models
{
    public class EmissionFactor
    {
        [Key]
        public int Kimlik { get; set; }
        public string YakitTipi { get; set; } = null!;
        public decimal KgCO2PerLitre { get; set; }
    }
}