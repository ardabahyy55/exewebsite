using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.Models;

public class SiteSetting
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Key { get; set; } = "";

    [StringLength(4000)]
    public string? Value { get; set; }
}