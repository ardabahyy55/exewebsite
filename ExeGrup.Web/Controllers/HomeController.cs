using ExeGrup.Web.Data;
using ExeGrup.Web.Services;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISettingsService _settings;

    public HomeController(AppDbContext db, ISettingsService settings)
    {
        _db = db;
        _settings = settings;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel
        {
            Services = await _db.Services.AsNoTracking()
                .Where(s => s.IsActive).OrderBy(s => s.SortOrder).Take(6).ToListAsync(),
            Projects = await _db.Projects.AsNoTracking()
                .Where(p => p.IsActive).OrderByDescending(p => p.Date).Take(6)
                .Include(p => p.Images).ToListAsync(),
            Gallery = await _db.GalleryImages.AsNoTracking()
                .OrderBy(g => g.SortOrder).ThenByDescending(g => g.UploadedAt).Take(8).ToListAsync()
        };
        return View(model);
    }

    [HttpGet("hakkimizda")]
    public IActionResult About()
    {
        ViewData["ActiveNav"] = "hakkimizda";
        ViewData["Title"] = "Hakkımızda | EXE GRUP — tabela.exe";
        ViewData["MetaDescription"] = "tabela.exe, EXE GRUP bünyesinde Samsun Atakum'da dış mekan tabela ve dış cephe reklam uygulamaları üreten bir reklam firmasıdır.";
        return View();
    }
}