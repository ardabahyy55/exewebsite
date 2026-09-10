using ExeGrup.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/dashboard")]
public class DashboardController : AdminBaseController
{
    public DashboardController(AppDbContext db) : base(db) { }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProjects = await Db.Projects.CountAsync();
        ViewBag.TotalServices = await Db.Services.CountAsync();
        ViewBag.TotalImages = await Db.GalleryImages.CountAsync() + await Db.ProjectImages.CountAsync();
        ViewBag.NewMessages = await Db.ContactMessages.CountAsync(m => !m.IsRead);
        ViewBag.RecentMessages = await Db.ContactMessages.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt).Take(6).ToListAsync();

        return View();
    }
}