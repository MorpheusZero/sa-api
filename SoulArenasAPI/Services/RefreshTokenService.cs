using SoulArenasAPI.Database;
using SoulArenasAPI.Database.Entities;

namespace SoulArenasAPI.Services
{
    public class RefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext;
        public RefreshTokenService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshTokenEntity> UpdateRefreshToken(RefreshTokenEntity refreshTokenEntity)
        {
            _dbContext.RefreshTokens.Update(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return refreshTokenEntity;
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenById(int refreshTokenId)
        {
            return await _dbContext.RefreshTokens.FindAsync(refreshTokenId);
        }

        public async Task<RefreshTokenEntity> CreateRefreshToken(
            int userId,
            string refreshTokenHash,
            string deviceInfo,
            string ipAddress,
            string userAgent
        )
        {
            var refreshTokenEntity = new RefreshTokenEntity
            {
                UserId = userId,
                TokenHash = refreshTokenHash,
                DeviceInfo = deviceInfo,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return refreshTokenEntity;
        }
    }
}
