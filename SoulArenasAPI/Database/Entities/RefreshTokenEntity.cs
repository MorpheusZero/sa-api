namespace SoulArenasAPI.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("refresh_tokens")]
public class RefreshTokenEntity : AppBaseEntity
{
    public DateTime? RevokedAt { get; set; } = null;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; } = null;

    public string UserAgent { get; set; } = string.Empty;

    public string IPAddress { get; set; } = string.Empty;

    public string DeviceInfo { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public UserEntity User { get; set; } = null!;
}