using ExeGrup.Web.Data;
using ExeGrup.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    [HttpGet("projeler")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActiveNav"] = "projeler";
        ViewData["Title"] = "Projeler | tabela.exe — EXE GRUP";
        ViewData["MetaDescription"] = "Samsun'da tamamlanan tabela ve dış cephe reklam projelerimiz: kompozit kutu harf, ışıklı tabela ve kurumsal cephe uygulamaları.";
        var projects = await _db.Projects.AsNoTracking()
            .Where(p => p.IsActive).OrderByDescending(p => p.Date).ToListAsync();
        return View(projects);
    }

    [HttpGet("projeler/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var project = await _db.Projects.AsNoTracking()
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
        if (project is null) return NotFound();

        ViewData["ActiveNav"] = "projeler";
        ViewData["Title"] = $"{project.Name} | tabela.exe Projesi";
        ViewData["MetaDescription"] = project.ShortDescription;
        ViewData["OgImage"] = project.CoverImage;

        ViewBag.Related = await _db.Projects.AsNoTracking()
            .Where(p => p.IsActive && p.Id != project.Id && p.Category == project.Category)
            .OrderByDescending(p => p.Date).Take(3).ToListAsync();

        return View(project);
    }
}