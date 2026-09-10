using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class Project
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [Required, StringLength(220)]
    public string Slug { get; set; } = "";

    [Required, StringLength(100)]
    public string Category { get; set; } = "";

    [Required, StringLength(500)]
    public string ShortDescription { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [StringLength(300)]
    public string? CoverImage { get; set; }

    [StringLength(200)]
    public string? Material { get; set; }

    [StringLength(200)]
    public string? ApplicationType { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public bool IsActive { get; set; } = true;

    public List<ProjectImage> Images { get; set; } = new();
}