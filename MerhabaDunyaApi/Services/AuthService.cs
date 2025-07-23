using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace MerhabaDunyaApi.Services
{
    public interface IAuthService
    {
        Task<Kullanici> Register(Kullanici kullanici, string sifre);
        Task<string> Login(string eposta, string sifre);
        Task<bool> UserExists(string eposta);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Kullanici> Register(Kullanici kullanici, string sifre)
        {
            if (await UserExists(kullanici.EPosta))
                throw new Exception("Bu e-posta zaten kayıtlı");

            CreatePasswordHash(sifre, out byte[] sifreHash, out byte[] sifreSalt);

            kullanici.SifreHash = Convert.ToBase64String(sifreHash);
            kullanici.SifreSalt = Convert.ToBase64String(sifreSalt);
            kullanici.OlusturmaTarihi = DateTime.UtcNow;
            kullanici.Aktif = true;

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();

            return kullanici;
        }

        public async Task<string> Login(string eposta, string sifre)
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(x => x.EPosta == eposta && x.Aktif);

            if (kullanici == null || !VerifyPasswordHash(sifre,
                Convert.FromBase64String(kullanici.SifreHash),
                Convert.FromBase64String(kullanici.SifreSalt)))
            {
                throw new Exception("Geçersiz kullanıcı adı veya şifre");
            }

            kullanici.SonGirisTarihi = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GenerateJwtToken(kullanici);
        }

        public async Task<bool> UserExists(string eposta)
        {
            return await _context.Kullanicilar.AnyAsync(x => x.EPosta == eposta);
        }

        private void CreatePasswordHash(string sifre, out byte[] sifreHash, out byte[] sifreSalt)
        {
            using var hmac = new HMACSHA512();
            sifreSalt = hmac.Key;
            sifreHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sifre));
        }

        private bool VerifyPasswordHash(string sifre, byte[] sifreHash, byte[] sifreSalt)
        {
            using var hmac = new HMACSHA512(sifreSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sifre));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != sifreHash[i])
                    return false;
            }
            return true;
        }

        private string GenerateJwtToken(Kullanici kullanici)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, kullanici.Kimlik.ToString()),
                    new Claim(ClaimTypes.Email, kullanici.EPosta),
                    new Claim(ClaimTypes.Name, kullanici.AdSoyad)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}