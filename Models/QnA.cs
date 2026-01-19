using System.ComponentModel.DataAnnotations;

namespace ERP_QNA_WebApp.Models;

public class QnA
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Tags { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
