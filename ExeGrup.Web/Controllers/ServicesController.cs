using ExeGrup.Web.Data;
using ExeGrup.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Controllers;

public class ServicesController : Controller
{
    private readonly AppDbContext _db;
    public ServicesController(AppDbContext db) => _db = db;

    [HttpGet("hizmetler")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActiveNav"] = "hizmetler";
        ViewData["Title"] = "Hizmetler | Samsun Tabela ve Reklam — tabela.exe";
        ViewData["MetaDescription"] = "Dış mekan tabela, kompozit kutu harf, ışıklı tabela, kurumsal tabela ve cephe tabela hizmetleri. Samsun ve Atakum'da üretim ve montaj.";
        var services = await _db.Services.AsNoTracking()
            .Where(s => s.IsActive).OrderBy(s => s.SortOrder).ToListAsync();
        return View(services);
    }

    // /dis-mekan-tabela, /kompozit-kutu-harf, /isikli-tabela vb.
    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var service = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive);
        if (service is null) return NotFound();

        ViewData["ActiveNav"] = "hizmetler";
        ViewData["Title"] = string.IsNullOrWhiteSpace(service.SeoTitle) ? $"{service.Title} | tabela.exe" : service.SeoTitle;
        ViewData["MetaDescription"] = service.SeoDescription ?? service.ShortDescription;
        ViewData["OgImage"] = service.Image;

        ViewBag.Others = await _db.Services.AsNoTracking()
            .Where(s => s.IsActive && s.Id != service.Id).OrderBy(s => s.SortOrder)
            .Take(3).ToListAsync();

        return View(service);
    }
}