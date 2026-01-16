using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SoulArenasAPI.Database.Entities;

[Table("users")]
[Index(nameof(Email), IsUnique = true)]
public class UserEntity : AppBaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string Username { get; set; } = string.Empty;
}