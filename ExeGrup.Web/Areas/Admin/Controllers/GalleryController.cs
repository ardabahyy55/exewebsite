using ExeGrup.Web.Data;
using ExeGrup.Web.Models;
using ExeGrup.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/galeri")]
public class GalleryController : AdminBaseController
{
    private readonly IFileUploadService _uploads;

    public GalleryController(AppDbContext db, IFileUploadService uploads) : base(db)
        => _uploads = uploads;

    [HttpGet("")]
    public async Task<IActionResult> Index()
        => View(await Db.GalleryImages.AsNoTracking()
            .OrderBy(g => g.SortOrder).ThenByDescending(g => g.UploadedAt).ToListAsync());

    [HttpPost("yukle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(List<IFormFile> files, string category, string? title)
    {
        var categorySafe = string.IsNullOrWhiteSpace(category) ? "Dış Mekan Tabela" : category.Trim();
        var added = 0;
        try
        {
            foreach (var file in files ?? new List<IFormFile>())
            {
                var path = await _uploads.SaveImageAsync(file, "gallery");
                if (path is null) continue;
                Db.GalleryImages.Add(new GalleryImage
                {
                    ImagePath = path,
                    Category = categorySafe,
                    Title = title?.Trim(),
                    SortOrder = 0,
                    UploadedAt = DateTime.Now
                });
                added++;
            }
            await Db.SaveChangesAsync();
            TempData["Success"] = added > 0 ? $"{added} görsel galeriye eklendi." : "Yüklenecek geçerli dosya bulunamadı.";
        }
        catch (InvalidOperationException ex)
        {
            await Db.SaveChangesAsync();
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("sil/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var image = await Db.GalleryImages.FindAsync(id);
        if (image is null) return NotFound();
        _uploads.DeleteImage(image.ImagePath);
        Db.GalleryImages.Remove(image);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Galeri görseli silindi.";
        return RedirectToAction(nameof(Index));
    }
}