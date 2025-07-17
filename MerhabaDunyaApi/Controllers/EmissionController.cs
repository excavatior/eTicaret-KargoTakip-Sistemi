// MerhabaDunyaApi/Controllers/EmissionController.cs
using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmissionController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EmissionController(AppDbContext db) => _db = db;

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] EmissionRequest req)
        {
            // 1. Faktörü DB'den çek
            var factor = await _db.EmissionFactors
                                  .FirstOrDefaultAsync(e => e.YakitTipi == req.YakitTipi);
            if (factor == null)
                return BadRequest(new { Hata = "Böyle bir yakıt tipi bulunamadı." });

            // 2. Hesaplama: mesafe * kgCO2PerLitre
            decimal result = req.Mesafe * factor.KgCO2PerLitre;

            // 3. Dönüş modeliyle cevapla
            return Ok(new EmissionResponse { Emisyon = Math.Round(result, 2) });
        }
    }
}
