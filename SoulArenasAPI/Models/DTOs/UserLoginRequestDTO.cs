namespace SoulArenasAPI.Models.DTOs;

public class UserLoginRequestDTO
{

    public UserLoginRequestDTO()
    {
    }

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
