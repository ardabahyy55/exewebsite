using ExeGrup.Web.Models;

namespace ExeGrup.Web.ViewModels;

public class HomeIndexViewModel
{
    public List<Service> Services { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<GalleryImage> Gallery { get; set; } = new();
}