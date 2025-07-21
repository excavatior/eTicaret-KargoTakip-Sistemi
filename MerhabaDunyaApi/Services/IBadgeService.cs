namespace MerhabaDunyaApi.Services
{
    public interface IBadgeService
    {
        Task AwardBadgesAsync(int kullaniciId, decimal oldEmission, decimal newEmission);
    }
}