using SoulArenasAPI.Database;
using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Util;
using Microsoft.EntityFrameworkCore;

namespace SoulArenasAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity?> GetUserById(int userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<UserEntity?> GetUserByEmail(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<UserEntity> CreateUser(UserCreateRequestDTO userCreateRequest)
        {
            var userEntity = new UserEntity
            {
                Email = userCreateRequest.Email.ToLower(),
                Username = UsernameGenerator.GenerateUsername(),
                PasswordHash = await CryptoHelper.HashPasswordAsync(userCreateRequest.Password),
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                IsEmailVerified = false
            };

            _dbContext.Users.Add(userEntity);
            await _dbContext.SaveChangesAsync();

            return userEntity;
        }
    }
}