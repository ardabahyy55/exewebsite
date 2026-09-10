using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class GalleryImage
{
    public int Id { get; set; }

    [Required, StringLength(300)]
    public string ImagePath { get; set; } = "";

    [StringLength(200)]
    public string? Title { get; set; }

    [Required, StringLength(100)]
    public string Category { get; set; } = "Dış Mekan Tabela";

    public int SortOrder { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;
}