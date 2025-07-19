using MerhabaDunyaApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IEmissionCalculator _emissionCalc;
        private readonly IHttpClientFactory _httpClientFactory;

        public RouteController(IEmissionCalculator emissionCalc, IHttpClientFactory httpClientFactory)
        {
            _emissionCalc = emissionCalc;
            _httpClientFactory = httpClientFactory;
        }

        public class OptimizeRequest
        {
            public List<double[]> Waypoints { get; set; } = null!; // [lat,lon]
            public string Mode { get; set; } = "fast";            // "fast" veya "green"
            public string FuelType { get; set; } = "Diesel";
        }
        public class OptimizeResponse
        {
            public decimal TotalDistanceKm { get; set; }
            public decimal TotalDurationMin { get; set; }
            public decimal TotalEmissionKg { get; set; }
            public List<RouteSegment> Segments { get; set; } = null!;
        }
        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] OptimizeRequest req)
        {
            // 1) OSRM çağrısını dene
            var client = _httpClientFactory.CreateClient("routing");
            var coords = string.Join(";", req.Waypoints.Select(w => $"{w[1]},{w[0]}"));
            var url = $"route/v1/driving/{coords}?overview=false";

            List<RouteSegment> segments;
            double totalDistance;
            double totalDuration = 0; // tahmini

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var osrm = await response.Content.ReadFromJsonAsync<OsrmResponse>();
                var route = osrm!.Routes[0];
                totalDistance = route.Distance / 1000.0;
                totalDuration = route.Duration / 60.0;
                segments = route.Legs.SelectMany(leg =>
                    leg.Steps.Select(s =>
                        new RouteSegment
                        {
                            DistanceKm = (decimal)(s.Distance / 1000.0),
                            DurationMin = (decimal)(s.Duration / 60.0)
                        }))
                    .ToList();
            }
            catch
            {
                // 2) Fallback: sadece iki nokta arası kuş uçuşu mesafe (Haversine)
                var p1 = req.Waypoints[0];
                var p2 = req.Waypoints[1];
                totalDistance = Haversine((double)p1[0], (double)p1[1], (double)p2[0], (double)p2[1]);
                totalDuration = (totalDistance / 50.0) * 60.0;  // saatte 50 km/h varsayımı
                segments = new List<RouteSegment> {
            new RouteSegment {
                DistanceKm  = (decimal)totalDistance,
                DurationMin = (decimal)totalDuration
            }
        };
            }

            // 3) Emisyonu hesapla
            var totalEmission = _emissionCalc.CalculateTotalEmission(segments, req.FuelType);

            // 4) Yanıtı oluştur
            return Ok(new OptimizeResponse
            {
                TotalDistanceKm = Math.Round((decimal)totalDistance, 2),
                TotalDurationMin = Math.Round((decimal)totalDuration, 2),
                TotalEmissionKg = totalEmission,
                Segments = segments
            });
        }

        // Haversine formülü
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarıçapı km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double deg) => deg * (Math.PI / 180);

        // OSRM JSON modelleri (gerektiği kadar)
        public class OsrmResponse { public List<Route> Routes { get; set; } = new(); }
        public class Route { 
            public double Distance { get; set; } 
            public double Duration { get; set; } 
            public List<Leg> Legs { get; set; } = new(); 
        }
        public class Leg { public List<Step> Steps { get; set; } = new(); }
        public class Step { 
            public double Distance { get; set; } 
            public double Duration { get; set; } }
    }
}
