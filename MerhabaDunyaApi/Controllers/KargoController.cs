using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;
using MerhabaDunyaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KargoController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;

        public KargoController(AppDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        // GET: api/kargo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kargo>>> GetKargolar()
        {
            var kargolar = await _db.Kargolar.ToListAsync();
            return Ok(kargolar);
        }

        // GET: api/kargo/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Kargo>> GetKargo(int id)
        {
            var kargo = await _db.Kargolar.FindAsync(id);
            if (kargo == null) return NotFound();
            return Ok(kargo);
        }

        // POST: api/kargo
        [HttpPost]
        public async Task<ActionResult<Kargo>> CreateKargo([FromBody] Kargo kargo)
        {
            _db.Kargolar.Add(kargo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKargo), new { id = kargo.Kimlik }, kargo);
        }

        // PUT: api/kargo/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateKargo(int id, [FromBody] Kargo updated)
        {
            if (id != updated.Kimlik) return BadRequest();
            _db.Entry(updated).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/kargo/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteKargo(int id)
        {
            var kargo = await _db.Kargolar.FindAsync(id);
            if (kargo == null) return NotFound();
            _db.Kargolar.Remove(kargo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/kargo/{takipNo}/status
        [HttpPut("{takipNo}/status")]
        public async Task<IActionResult> UpdateStatus(string takipNo, [FromBody] string newStatus)
        {
            var kargo = await _db.Kargolar
                .Include(k => k.Siparis)
                .FirstOrDefaultAsync(k => k.TakipNumarasi == takipNo);

            if (kargo == null) return NotFound($"Takip numarası '{takipNo}' bulunamadı.");

            kargo.Durum = newStatus;
            await _db.SaveChangesAsync();

            // Bildirim gönder
            await _notificationService.SendNotificationAsync(
                kargo.Siparis.KullaniciKimlik,
                "Kargo Güncellemesi",
                $"Siparişinizin durumu artık: {newStatus}"
            );

            return NoContent();
        }
    }
}
