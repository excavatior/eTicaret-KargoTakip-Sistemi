using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MerhabaDunyaApi.Services;
using SvcSegment = MerhabaDunyaApi.Services.RouteSegment;  // Service-side RouteSegment

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IEmissionCalculator _emissionCalc;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBadgeService _badgeService;
        private readonly INotificationService _notificationService;

        public RouteController(
            IEmissionCalculator emissionCalc,
            IHttpClientFactory httpClientFactory,
            IBadgeService badgeService,
            INotificationService notificationService)
        {
            _emissionCalc = emissionCalc;
            _httpClientFactory = httpClientFactory;
            _badgeService = badgeService;
            _notificationService = notificationService;
        }

        public class OptimizeRequest
        {
            public int KullaniciId { get; set; }
            public decimal OncekiEmisyon { get; set; }  // kg CO₂
            public List<double[]> Waypoints { get; set; } = null!;
            public string Mode { get; set; } = "fast";
            public string FuelType { get; set; } = "Diesel";
        }

        public class OptimizeResponse
        {
            public decimal TotalDistanceKm { get; set; }
            public decimal TotalDurationMin { get; set; }
            public decimal TotalEmissionKg { get; set; }
            public List<SvcSegment> Segments { get; set; } = new();
        }

        // OSRM JSON modelleri (nested, internal)
        private class OsrmResponse { public List<Route> Routes { get; set; } = new(); }
        private class Route { public double Distance { get; set; } public double Duration { get; set; } public List<Leg> Legs { get; set; } = new(); }
        private class Leg { public List<Step> Steps { get; set; } = new(); }
        private class Step { public double Distance { get; set; } public double Duration { get; set; } }

        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] OptimizeRequest req)
        {
            // 1) OSRM çağrısı
            var client = _httpClientFactory.CreateClient("routing");
            var coords = string.Join(";", req.Waypoints.Select(w => $"{w[1]},{w[0]}"));
            var url = $"route/v1/driving/{coords}?overview=false";

            List<SvcSegment> segments;
            double totalDistance;
            double totalDuration;

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var osrm = await response.Content.ReadFromJsonAsync<OsrmResponse>();
                var route = osrm!.Routes.First();

                totalDistance = route.Distance / 1000.0;
                totalDuration = route.Duration / 60.0;
                segments = route.Legs
                    .SelectMany(leg => leg.Steps.Select(s => new SvcSegment
                    {
                        DistanceKm = (decimal)(s.Distance / 1000.0),
                        DurationMin = (decimal)(s.Duration / 60.0)
                    }))
                    .ToList();
            }
            catch
            {
                // 2) Fallback: Haversine
                var p1 = req.Waypoints[0];
                var p2 = req.Waypoints[1];
                totalDistance = Haversine(p1[0], p1[1], p2[0], p2[1]);
                totalDuration = (totalDistance / 50.0) * 60.0;
                segments = new List<SvcSegment> {
                    new SvcSegment {
                        DistanceKm  = (decimal)totalDistance,
                        DurationMin = (decimal)totalDuration
                    }
                };
            }

            // 3) Emisyon hesapla
            var totalEmission = _emissionCalc.CalculateTotalEmission(segments, req.FuelType);

            // 4) Rozet ver
            await _badgeService.AwardBadgesAsync(req.KullaniciId, req.OncekiEmisyon, totalEmission);

            // 5) Bildirim gönder
            var savePct = req.OncekiEmisyon > 0
                ? Math.Round((req.OncekiEmisyon - totalEmission) / req.OncekiEmisyon * 100M)
                : 0;
            await _notificationService.SendNotificationAsync(
                req.KullaniciId,
                "Yeşil Rota Hazır!",
                $"%{savePct} emisyon tasarrufu sağladınız."
            );

            // 6) Yanıtı döndür
            var resp = new OptimizeResponse
            {
                TotalDistanceKm = Math.Round((decimal)totalDistance, 2),
                TotalDurationMin = Math.Round((decimal)totalDuration, 2),
                TotalEmissionKg = totalEmission,
                Segments = segments
            };
            return Ok(resp);
        }

        // Haversine hesaplaması
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarıçapı km
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double deg) => deg * (Math.PI / 180);
    }
}
