using MerhabaDunyaApi.Models;
using System.Collections.Generic;
using System.Linq;
using MerhabaDunyaApi.Data;

namespace MerhabaDunyaApi.Services
{
    public interface IEmissionCalculator
    {
        decimal CalculateTotalEmission(IEnumerable<RouteSegment> segments, string yakitTipi);
    }

    public class EmissionCalculator : IEmissionCalculator
    {
        private readonly AppDbContext _db;

        public EmissionCalculator(AppDbContext db)
        {
            _db = db;
        }

        public decimal CalculateTotalEmission(IEnumerable<RouteSegment> segments, string yakitTipi)
        {
            var factor = _db.EmissionFactors
                            .FirstOrDefault(e => e.YakitTipi == yakitTipi)
                        ?? throw new KeyNotFoundException($"Yakıt tipi '{yakitTipi}' bulunamadı.");

            var totalKm = segments.Sum(s => s.DistanceKm);
            return Math.Round(totalKm * factor.KgCO2PerLitre, 2);
        }
    }

    // Rota servisiyle anlaşacağımız model
    public class RouteSegment
    {
        public decimal DistanceKm { get; set; }
        public decimal DurationMin { get; set; }
    }
}
