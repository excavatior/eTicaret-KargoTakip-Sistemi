using MerhabaDunyaApi.DTOs;
using MerhabaDunyaApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using MerhabaDunyaApi.Models;
namespace MerhabaDunyaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(KullaniciKayitDto kullaniciKayitDto)
        {
            if (await _authService.UserExists(kullaniciKayitDto.EPosta))
                return BadRequest("Bu e-posta zaten kayıtlı");

            var kullanici = new Kullanici
            {
                AdSoyad = kullaniciKayitDto.AdSoyad,
                EPosta = kullaniciKayitDto.EPosta
            };

            var createdUser = await _authService.Register(kullanici, kullaniciKayitDto.Sifre);

            return StatusCode(201, new
            {
                createdUser.Kimlik,
                createdUser.AdSoyad,
                createdUser.EPosta
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(KullaniciGirisDto kullaniciGirisDto)
        {
            var token = await _authService.Login(kullaniciGirisDto.EPosta, kullaniciGirisDto.Sifre);

            if (token == null)
                return Unauthorized("Geçersiz kullanıcı adı veya şifre");

            return Ok(new { token });
        }
    }
}