using System.ComponentModel;
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

    [Required]
    [DefaultValue(false)]
    public bool IsEmailVerified { get; set; } = false;

    [Required]
    [MaxLength(1024)]
    public string RolesRaw { get; set; } = string.Empty;

    [NotMapped]
    public HashSet<string> Permissions { get; set; } = new();
}
