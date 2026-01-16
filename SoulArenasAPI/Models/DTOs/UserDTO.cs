using SoulArenasAPI.Database.Entities;

namespace SoulArenasAPI.Models.DTOs;

public class UserDTO
{

    public UserDTO()
    {
    }

    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public void fromEntity(UserEntity entity)
    {
        Id = entity.Id;
        CreatedAt = entity.CreatedAt;
        LastModified = entity.LastModified;
        IsDeleted = entity.IsDeleted;
        IsActive = entity.IsActive;
        Email = entity.Email;
        Username = entity.Username;
    }
}