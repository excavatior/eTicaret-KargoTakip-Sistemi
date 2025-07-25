using System.Threading.Tasks;
using MerhabaDunyaApi.Models;

namespace MerhabaDunyaApi.Services
{
    public interface IAuthService
    {
        Task<Kullanici> Register(Kullanici kullanici, string sifre);
        Task<LoginResult> Login(string eposta, string sifre);
        Task<bool> UserExists(string eposta);
        Task Logout(string userId);
        Task<Kullanici?> GetUserById(string userId);
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } = 3600;
        public Kullanici User { get; set; } = default!;
    }
}
