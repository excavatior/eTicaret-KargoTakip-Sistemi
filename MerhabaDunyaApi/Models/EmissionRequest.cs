
namespace MerhabaDunyaApi.Models
{
    public class EmissionRequest
    {
        public string YakitTipi { get; set; } = null!;
        public decimal Mesafe { get; set; }     // kilometre cinsinden
    }
}


namespace MerhabaDunyaApi.Models
{
    public class EmissionResponse
    {
        public decimal Emisyon { get; set; }    // kg CO2
    }
}
