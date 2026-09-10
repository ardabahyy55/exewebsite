using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class ContactMessage
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; } = "";

    [Required, StringLength(30)]
    public string Phone { get; set; } = "";

    [Required, StringLength(200)]
    public string Email { get; set; } = "";

    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(300)]
    public string? Subject { get; set; }

    [Required]
    public string Message { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsRead { get; set; }
}