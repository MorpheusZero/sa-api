using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Util;

namespace SoulArenasAPI.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly RefreshTokenService _refreshTokenService;
        public AuthService(UserService userService, RefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<UserDTO> RegisterNewUser(UserCreateRequestDTO userCreateRequest)
        {
            var existingUser = await _userService.GetUserByEmail(userCreateRequest.Email);
            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }

            var userEntity = await _userService.CreateUser(userCreateRequest);

            return new UserDTO().fromEntity(userEntity);
        }

        public async Task<UserDTO> LoginUser(UserLoginRequestDTO userLoginRequest)
        {
            var userEntity = await _userService.GetUserByEmail(userLoginRequest.Email);
            if (userEntity == null)
            {
                throw new Exception("Invalid email or password.");
            }

            var isPasswordValid = await CryptoHelper.VerifyPasswordAsync(userLoginRequest.Password, userEntity.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password.");
            }

            return new UserDTO().fromEntity(userEntity);
        }

        public async Task<string> GenerateRefreshTokenForUser(int userId, string deviceInfo, string ipAddress, string userAgent)
        {
            var randomString = StringHelper.GenerateRandomStringWithSize(128);
            var hashedRefreshToken = await CryptoHelper.HashPasswordAsync(randomString);

            var refreshTokenEntity = await _refreshTokenService.CreateRefreshToken(
                userId,
                hashedRefreshToken,
                deviceInfo,
                ipAddress,
                userAgent
            );

            return refreshTokenEntity.Id.ToString() + "." + randomString;
        }

        public async Task<UserEntity?> ValidateRefreshToken(int refreshTokenId, string providedToken)
        {
            var refreshTokenEntity = await _refreshTokenService.GetRefreshTokenById(refreshTokenId);
            if (refreshTokenEntity == null || refreshTokenEntity.RevokedAt != null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            var isTokenValid = await CryptoHelper.VerifyPasswordAsync(providedToken, refreshTokenEntity.TokenHash);
            if (!isTokenValid)
            {
                return null;
            }

            refreshTokenEntity.RevokedAt = DateTime.UtcNow;
            refreshTokenEntity.LastUsedAt = DateTime.UtcNow;
            refreshTokenEntity.LastModified = DateTime.UtcNow;

            refreshTokenEntity = await _refreshTokenService.UpdateRefreshToken(refreshTokenEntity);

            return refreshTokenEntity.User;
        }
    }
}
