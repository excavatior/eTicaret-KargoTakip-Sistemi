using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MerhabaDunyaApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<Kullanici> Register(Kullanici kullanici, string sifre)
        {
            if (await UserExists(kullanici.EPosta))
                throw new InvalidOperationException("Bu e‑posta zaten kayıtlı.");

            // Şifreye salt ve hash üret
            CreatePasswordHash(sifre, out var hash, out var salt);

            kullanici.SifreHash = Convert.ToBase64String(hash);
            kullanici.SifreSalt = salt;              // >> byte[]
            kullanici.OlusturmaTarihi = DateTime.UtcNow;
            kullanici.Aktif = true;

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();
            return kullanici;
        }

        public async Task<LoginResult> Login(string eposta, string sifre)
        {
            var user = await _context.Kullanicilar
                .FirstOrDefaultAsync(u => u.EPosta == eposta && u.Aktif);

            // VerifyPasswordHash, Base64 hash ve byte[] salt’la çalışıyor:
            if (user == null
                || !VerifyPasswordHash(sifre, user.SifreHash, user.SifreSalt))
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Geçersiz e‑posta veya şifre."
                };
            }

            user.SonGirisTarihi = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginResult
            {
                Success = true,
                Token = GenerateJwtToken(user),
                ExpiresIn = 3600,
                User = user
            };
        }

        public Task<bool> UserExists(string eposta)
            => _context.Kullanicilar.AnyAsync(u => u.EPosta == eposta);

        public Task Logout(string userId)
            => Task.CompletedTask;

        public async Task<Kullanici?> GetUserById(string userId)
        {
            if (!int.TryParse(userId, out var id)) return null;
            return await _context.Kullanicilar
                .FirstOrDefaultAsync(u => u.Kimlik == id);
        }

        // ----- Yardımcı metotlar -----

        private void CreatePasswordHash(string sifre, out byte[] hash, out byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(sifre))
                throw new ArgumentException("Şifre boş olamaz.", nameof(sifre));

            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sifre));
        }

        private bool VerifyPasswordHash(string sifre, string storedHashBase64, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(sifre)
             || string.IsNullOrWhiteSpace(storedHashBase64)
             || storedSalt == null || storedSalt.Length == 0)
                return false;

            var hashBytes = Convert.FromBase64String(storedHashBase64);
            using var hmac = new HMACSHA512(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(sifre));
            return computed.AsSpan().SequenceEqual(hashBytes);
        }

        private string GenerateJwtToken(Kullanici user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key),
                                                   SecurityAlgorithms.HmacSha512);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Kimlik.ToString()),
                new Claim(ClaimTypes.Email, user.EPosta),
                new Claim(ClaimTypes.Name, user.AdSoyad)
            };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
