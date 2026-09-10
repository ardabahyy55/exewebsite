using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class AdminUser
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Username { get; set; } = "";

    [Required, StringLength(500)]
    public string PasswordHash { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}