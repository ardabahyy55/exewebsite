using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class ProjectImage
{
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    [Required, StringLength(300)]
    public string ImagePath { get; set; } = "";

    public int SortOrder { get; set; }
}