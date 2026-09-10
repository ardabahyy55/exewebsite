using ExeGrup.Web.Data;
using ExeGrup.Web.Models;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExeGrup.Web.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;
    public ContactController(AppDbContext db) => _db = db;

    [HttpGet("iletisim")]
    public IActionResult Index()
    {
        ViewData["ActiveNav"] = "iletisim";
        ViewData["Title"] = "İletişim | tabela.exe — Samsun Tabela";
        ViewData["MetaDescription"] = "Samsun Atakum'da tabela teklifi alın: 0540 550 52 55, info@exegrup.com.tr. WhatsApp'tan yazın, en kısa sürede dönüş yapalım.";
        return View(new ContactFormViewModel());
    }

    [HttpPost("iletisim")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ActiveNav"] = "iletisim";
            ViewData["Title"] = "İletişim | tabela.exe — Samsun Tabela";
            return View(vm);
        }

        _db.ContactMessages.Add(new ContactMessage
        {
            FullName = vm.FullName.Trim(),
            Phone = vm.Phone.Trim(),
            Email = vm.Email.Trim(),
            Company = vm.Company?.Trim(),
            Subject = vm.Subject?.Trim(),
            Message = vm.Message.Trim(),
            CreatedAt = DateTime.Now,
            IsRead = false
        });
        await _db.SaveChangesAsync();

        TempData["ContactSuccess"] = true;
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }
}