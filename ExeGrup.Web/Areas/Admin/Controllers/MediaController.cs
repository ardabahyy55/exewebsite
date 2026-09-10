using ExeGrup.Web.Data;
using ExeGrup.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/medya")]
public class MediaController : AdminBaseController
{
    private readonly IFileUploadService _uploads;
    private readonly IWebHostEnvironment _env;

    public MediaController(AppDbContext db, IFileUploadService uploads, IWebHostEnvironment env) : base(db)
    {
        _uploads = uploads;
        _env = env;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.Files = ListUploads();
        return View();
    }

    [HttpPost("yukle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        var added = 0;
        try
        {
            foreach (var file in files ?? new List<IFormFile>())
            {
                if (await _uploads.SaveImageAsync(file, "media") is not null) added++;
            }
            TempData["Success"] = added > 0 ? $"{added} dosya yüklendi." : "Yüklenecek geçerli dosya bulunamadı.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("sil")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/uploads/"))
            return BadRequest("Geçersiz dosya yolu.");
        _uploads.DeleteImage(path);
        TempData["Success"] = "Dosya silindi.";
        return RedirectToAction(nameof(Index));
    }

    private List<string> ListUploads()
    {
        var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        if (!Directory.Exists(root)) return new List<string>();
        return Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
            .Select(f => "/uploads/" + Path.GetRelativePath(root, f).Replace("\\", "/"))
            .OrderByDescending(f => f)
            .ToList();
    }
}