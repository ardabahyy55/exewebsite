using ExeGrup.Web.Data;
using ExeGrup.Web.Helpers;
using ExeGrup.Web.Models;
using ExeGrup.Web.Services;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/projeler")]
public class ProjectsController : AdminBaseController
{
    private readonly IFileUploadService _uploads;

    public ProjectsController(AppDbContext db, IFileUploadService uploads) : base(db)
        => _uploads = uploads;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var projects = await Db.Projects.AsNoTracking()
            .OrderByDescending(p => p.Date).ThenByDescending(p => p.Id).ToListAsync();
        return View(projects);
    }

    [HttpGet("yeni")]
    public IActionResult Create() => View("Form", new ProjectFormViewModel());

    [HttpPost("yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel vm)
    {
        vm.Slug = SlugHelper.Slugify(vm.Slug);
        if (await Db.Projects.AnyAsync(p => p.Slug == vm.Slug))
            ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

        if (!ModelState.IsValid) return View("Form", vm);

        try
        {
            var project = new Project
            {
                Name = vm.Name.Trim(),
                Slug = vm.Slug,
                Category = vm.Category.Trim(),
                ShortDescription = vm.ShortDescription.Trim(),
                Description = vm.Description.Trim(),
                Material = vm.Material?.Trim(),
                ApplicationType = vm.ApplicationType?.Trim(),
                Date = vm.Date,
                IsActive = vm.IsActive
            };

            project.CoverImage = await _uploads.SaveImageAsync(vm.CoverImageFile, "projects");

            if (vm.ImageFiles is not null)
            {
                foreach (var file in vm.ImageFiles)
                {
                    var path = await _uploads.SaveImageAsync(file, "projects");
                    if (path is not null)
                        project.Images.Add(new ProjectImage { ImagePath = path, SortOrder = project.Images.Count });
                }
            }

            Db.Projects.Add(project);
            await Db.SaveChangesAsync();
            TempData["Success"] = "Proje eklendi.";
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
        var project = await Db.Projects.Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();

        return View("Form", new ProjectFormViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            Category = project.Category,
            ShortDescription = project.ShortDescription,
            Description = project.Description,
            CoverImage = project.CoverImage,
            Material = project.Material,
            ApplicationType = project.ApplicationType,
            Date = project.Date,
            IsActive = project.IsActive,
            Images = project.Images.ToList()
        });
    }

    [HttpPost("duzenle/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        vm.Slug = SlugHelper.Slugify(vm.Slug);
        if (await Db.Projects.AnyAsync(p => p.Slug == vm.Slug && p.Id != id))
            ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

        if (!ModelState.IsValid)
        {
            vm.Images = await Db.ProjectImages.Where(i => i.ProjectId == id)
                .OrderBy(i => i.SortOrder).ToListAsync();
            return View("Form", vm);
        }

        var project = await Db.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();

        try
        {
            project.Name = vm.Name.Trim();
            project.Slug = vm.Slug;
            project.Category = vm.Category.Trim();
            project.ShortDescription = vm.ShortDescription.Trim();
            project.Description = vm.Description.Trim();
            project.Material = vm.Material?.Trim();
            project.ApplicationType = vm.ApplicationType?.Trim();
            project.Date = vm.Date;
            project.IsActive = vm.IsActive;

            var cover = await _uploads.SaveImageAsync(vm.CoverImageFile, "projects");
            if (cover is not null) project.CoverImage = cover;

            if (vm.ImageFiles is not null)
            {
                foreach (var file in vm.ImageFiles)
                {
                    var path = await _uploads.SaveImageAsync(file, "projects");
                    if (path is not null)
                        project.Images.Add(new ProjectImage { ProjectId = id, ImagePath = path, SortOrder = project.Images.Count });
                }
            }

            await Db.SaveChangesAsync();
            TempData["Success"] = "Proje güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            vm.Images = await Db.ProjectImages.Where(i => i.ProjectId == id).OrderBy(i => i.SortOrder).ToListAsync();
            return View("Form", vm);
        }
    }

    [HttpPost("gorsel-sil/{imageId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await Db.ProjectImages.Include(i => i.Project).FirstOrDefaultAsync(i => i.Id == imageId);
        if (image is null) return NotFound();

        _uploads.DeleteImage(image.ImagePath);
        Db.ProjectImages.Remove(image);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Görsel silindi.";
        return RedirectToAction(nameof(Edit), new { id = image.ProjectId });
    }

    [HttpPost("durum/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var project = await Db.Projects.FindAsync(id);
        if (project is null) return NotFound();
        project.IsActive = !project.IsActive;
        await Db.SaveChangesAsync();
        TempData["Success"] = project.IsActive ? "Proje yayına alındı." : "Proje pasife alındı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("sil/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await Db.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();

        _uploads.DeleteImage(project.CoverImage);
        foreach (var img in project.Images) _uploads.DeleteImage(img.ImagePath);

        Db.Projects.Remove(project);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Proje silindi.";
        return RedirectToAction(nameof(Index));
    }
}