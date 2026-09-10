using ExeGrup.Web.Data;
using ExeGrup.Web.Services;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/ayarlar")]
public class SettingsController : AdminBaseController
{
    private readonly IFileUploadService _uploads;
    private readonly ISettingsService _settings;

    public SettingsController(AppDbContext db, IFileUploadService uploads, ISettingsService settings) : base(db)
    {
        _uploads = uploads;
        _settings = settings;
    }

    [HttpGet("")]
    public IActionResult Index() => View(BuildVm());

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(BuildVm(vm));

        await _settings.SetAsync("CompanyName", vm.CompanyName ?? "");
        await _settings.SetAsync("CompanySubBrand", vm.CompanySubBrand ?? "");
        await _settings.SetAsync("FooterText", vm.FooterText ?? "");
        await _settings.SetAsync("PhoneDisplay", vm.PhoneDisplay ?? "");
        await _settings.SetAsync("PhoneLink", vm.PhoneLink ?? "");
        await _settings.SetAsync("WhatsAppLink", vm.WhatsAppLink ?? "");
        await _settings.SetAsync("WhatsAppMessage", vm.WhatsAppMessage ?? "");
        await _settings.SetAsync("Email", vm.Email ?? "");
        await _settings.SetAsync("Address", vm.Address ?? "");
        await _settings.SetAsync("Instagram", vm.Instagram ?? "");
        await _settings.SetAsync("MapsEmbed", vm.MapsEmbed ?? "");
        await _settings.SetAsync("SiteUrl", vm.SiteUrl ?? "");
        await _settings.SetAsync("SeoTitle", vm.SeoTitle ?? "");
        await _settings.SetAsync("SeoDescription", vm.SeoDescription ?? "");

        try
        {
            var logo = await _uploads.SaveImageAsync(vm.LogoFile, "site");
            if (logo is not null) await _settings.SetAsync("LogoPath", logo);

            var favicon = await _uploads.SaveImageAsync(vm.FaviconFile, "site");
            if (favicon is not null) await _settings.SetAsync("FaviconPath", favicon);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(BuildVm(vm));
        }

        TempData["Success"] = "Site ayarları güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private SettingsFormViewModel BuildVm(SettingsFormViewModel? vm = null) => vm ?? new SettingsFormViewModel
    {
        CompanyName = _settings.Get("CompanyName"),
        CompanySubBrand = _settings.Get("CompanySubBrand"),
        FooterText = _settings.Get("FooterText"),
        PhoneDisplay = _settings.Get("PhoneDisplay"),
        PhoneLink = _settings.Get("PhoneLink"),
        WhatsAppLink = _settings.Get("WhatsAppLink"),
        WhatsAppMessage = _settings.Get("WhatsAppMessage"),
        Email = _settings.Get("Email"),
        Address = _settings.Get("Address"),
        Instagram = _settings.Get("Instagram"),
        MapsEmbed = _settings.Get("MapsEmbed"),
        SiteUrl = _settings.Get("SiteUrl"),
        SeoTitle = _settings.Get("SeoTitle"),
        SeoDescription = _settings.Get("SeoDescription")
    };
}