using ExeGrup.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Controllers;

public class GalleryController : Controller
{
    private readonly AppDbContext _db;
    public GalleryController(AppDbContext db) => _db = db;

    [HttpGet("galeri")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActiveNav"] = "galeri";
        ViewData["Title"] = "Galeri | tabela.exe — EXE GRUP";
        ViewData["MetaDescription"] = "Samsun tabela projelerinden görseller: dış mekan tabela, kompozit, kutu harf, ışıklı tabela ve cephe uygulamaları.";
        var images = await _db.GalleryImages.AsNoTracking()
            .OrderBy(g => g.SortOrder).ThenByDescending(g => g.UploadedAt).ToListAsync();
        return View(images);
    }
}