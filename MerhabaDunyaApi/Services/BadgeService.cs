using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;    // <-- Rozet, KullaniciRozetleri
using MerhabaDunyaApi.Services;  // <-- IBadgeService

namespace MerhabaDunyaApi.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly AppDbContext _db;

        public BadgeService(AppDbContext db) => _db = db;

        public async Task AwardBadgesAsync(int userId, decimal oldEmission, decimal newEmission)
        {
            if (oldEmission <= 0) return;

            var savePct = (oldEmission - newEmission) / oldEmission * 100M;

            var eligible = await _db.Rozetler
                .Where(r => r.RequiredSavePct <= savePct)
                .ToListAsync();

            foreach (var rozet in eligible)
            {
                var exists = await _db.KullaniciRozetleri
                    .AnyAsync(kr => kr.KullaniciKimlik == userId && kr.RozetId == rozet.Id);

                if (!exists)
                {
                    _db.KullaniciRozetleri.Add(new KullaniciRozetleri
                    {
                        KullaniciKimlik = userId,
                        RozetId = rozet.Id
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
