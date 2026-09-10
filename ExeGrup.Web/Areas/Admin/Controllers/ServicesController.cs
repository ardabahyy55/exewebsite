using ExeGrup.Web.Data;
using ExeGrup.Web.Helpers;
using ExeGrup.Web.Models;
using ExeGrup.Web.Services;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/hizmetler")]
public class ServicesController : AdminBaseController
{
    private readonly IFileUploadService _uploads;

    public ServicesController(AppDbContext db, IFileUploadService uploads) : base(db)
        => _uploads = uploads;

    [HttpGet("")]
    public async Task<IActionResult> Index()
        => View(await Db.Services.AsNoTracking().OrderBy(s => s.SortOrder).ThenBy(s => s.Id).ToListAsync());

    [HttpGet("yeni")]
    public IActionResult Create() => View("Form", new ServiceFormViewModel());

    [HttpPost("yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceFormViewModel vm)
    {
        vm.Slug = SlugHelper.Slugify(vm.Slug);
        if (await Db.Services.AnyAsync(s => s.Slug == vm.Slug))
            ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

        if (!ModelState.IsValid) return View("Form", vm);

        try
        {
            var service = new Service
            {
                Title = vm.Title.Trim(),
                Slug = vm.Slug,
                ShortDescription = vm.ShortDescription.Trim(),
                Description = vm.Description.Trim(),
                SeoTitle = vm.SeoTitle?.Trim(),
                SeoDescription = vm.SeoDescription?.Trim(),
                SortOrder = vm.SortOrder,
                IsActive = vm.IsActive
            };
            service.Image = await _uploads.SaveImageAsync(vm.ImageFile, "services");

            Db.Services.Add(service);
            await Db.SaveChangesAsync();
            TempData["Success"] = "Hizmet eklendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Form", vm);
        }
    }

    [HttpGet("duzenle/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await Db.Services.FindAsync(id);
        if (service is null) return NotFound();

        return View("Form", new ServiceFormViewModel
        {
            Id = service.Id, Title = service.Title, Slug = service.Slug,
            ShortDescription = service.ShortDescription, Description = service.Description,
            Image = service.Image, SeoTitle = service.SeoTitle, SeoDescription = service.SeoDescription,
            SortOrder = service.SortOrder, IsActive = service.IsActive
        });
    }

    [HttpPost("duzenle/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        vm.Slug = SlugHelper.Slugify(vm.Slug);
        if (await Db.Services.AnyAsync(s => s.Slug == vm.Slug && s.Id != id))
            ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

        if (!ModelState.IsValid) return View("Form", vm);

        var service = await Db.Services.FindAsync(id);
        if (service is null) return NotFound();

        try
        {
            service.Title = vm.Title.Trim();
            service.Slug = vm.Slug;
            service.ShortDescription = vm.ShortDescription.Trim();
            service.Description = vm.Description.Trim();
            service.SeoTitle = vm.SeoTitle?.Trim();
            service.SeoDescription = vm.SeoDescription?.Trim();
            service.SortOrder = vm.SortOrder;
            service.IsActive = vm.IsActive;

            var image = await _uploads.SaveImageAsync(vm.ImageFile, "services");
            if (image is not null) service.Image = image;

            await Db.SaveChangesAsync();
            TempData["Success"] = "Hizmet güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Form", vm);
        }
    }

    [HttpPost("durum/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var service = await Db.Services.FindAsync(id);
        if (service is null) return NotFound();
        service.IsActive = !service.IsActive;
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("sil/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await Db.Services.FindAsync(id);
        if (service is null) return NotFound();
        _uploads.DeleteImage(service.Image);
        Db.Services.Remove(service);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Hizmet silindi.";
        return RedirectToAction(nameof(Index));
    }
}