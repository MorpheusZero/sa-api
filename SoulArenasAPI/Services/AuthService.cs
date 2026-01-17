using SoulArenasAPI.Database.Entities;
using SoulArenasAPI.Models.DTOs;
using SoulArenasAPI.Util;

namespace SoulArenasAPI.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        public AuthService(UserService userService)
        {
            _userService = userService;
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
    }
}