namespace SoulArenasAPI.Models.DTOs;

public class UserCreateRequestDTO
{

    public UserCreateRequestDTO()
    {
    }

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}