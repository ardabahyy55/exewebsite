using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class Service
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = "";

    [Required, StringLength(170)]
    public string Slug { get; set; } = "";

    [Required, StringLength(400)]
    public string ShortDescription { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [StringLength(300)]
    public string? Image { get; set; }

    [StringLength(200)]
    public string? SeoTitle { get; set; }

    [StringLength(300)]
    public string? SeoDescription { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}