using SoulArenasAPI.Database;
using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Util;

namespace SoulArenasAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity> CreateUser(UserCreateRequestDTO userCreateRequest)
        {
            var userEntity = new UserEntity
            {
                Email = userCreateRequest.Email,
                Username = UsernameGenerator.GenerateUsername(),
                PasswordHash = await CryptoHelper.HashPasswordAsync(userCreateRequest.Password),
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _dbContext.Users.Add(userEntity);
            await _dbContext.SaveChangesAsync();

            return userEntity;
        }
    }
}