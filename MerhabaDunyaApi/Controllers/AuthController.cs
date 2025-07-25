using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MerhabaDunyaApi.DTOs;
using MerhabaDunyaApi.Models;
using MerhabaDunyaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MerhabaDunyaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] KullaniciKayitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _auth.UserExists(dto.EPosta))
                    return Conflict(new { Message = "E‑posta kullanımda." });

                var user = new Kullanici
                {
                    AdSoyad = dto.AdSoyad.Trim(),
                    EPosta = dto.EPosta.Trim().ToLower()
                };
                var created = await _auth.Register(user, dto.Sifre);

                return CreatedAtAction(nameof(GetCurrentUser), null, new
                {
                    created.Kimlik,
                    created.AdSoyad,
                    created.EPosta,
                    created.OlusturmaTarihi
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register error");
                return StatusCode(500, new { Message = "Kayıt hatası." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] KullaniciGirisDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.Login(dto.EPosta.Trim().ToLower(), dto.Sifre);
            if (!result.Success)
                return Unauthorized(new { result.Message });

            return Ok(new
            {
                result.Token,
                result.ExpiresIn,
                User = new { result.User.Kimlik, result.User.AdSoyad, result.User.EPosta }
            });
        }

        [Authorize, HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _auth.Logout(userId);
            return Ok(new { Message = "Çıkış yapıldı." });
        }

        [Authorize, HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _auth.GetUserById(userId);
            if (user == null) return NotFound(new { Message = "Kullanıcı bulunamadı." });
            return Ok(new
            {
                user.Kimlik,
                user.AdSoyad,
                user.EPosta,
                user.OlusturmaTarihi,
                user.SonGirisTarihi,
                user.Aktif
            });
        }
    }
}
