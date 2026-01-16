using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulArenasAPI.Database.Entities;

public abstract class AppBaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    
    [Required]
    public bool IsDeleted { get; set; } = false;
    
    [Required]
    public bool IsActive { get; set; } = true;
}