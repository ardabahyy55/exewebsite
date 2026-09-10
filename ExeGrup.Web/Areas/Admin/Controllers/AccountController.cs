using ExeGrup.Web.Data;
using ExeGrup.Web.Services;
using ExeGrup.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/hesap")]
public class AccountController : AdminBaseController
{
    public AccountController(AppDbContext db) : base(db) { }

    [HttpGet("")]
    public IActionResult Index() => View(new ChangeAccountViewModel());

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ChangeAccountViewModel vm)
    {
        var userId = int.Parse(User.FindFirst("AdminId")?.Value ?? "0");
        var user = await Db.AdminUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return NotFound();

        if (!PasswordHasher.Verify(vm.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError("CurrentPassword", "Mevcut şifre hatalı.");
            return View(vm);
        }

        var changedSomething = false;

        if (!string.IsNullOrWhiteSpace(vm.NewUsername) && vm.NewUsername.Trim() != user.Username)
        {
            var newUsername = vm.NewUsername.Trim();
            if (await Db.AdminUsers.AnyAsync(u => u.Username == newUsername && u.Id != userId))
            {
                ModelState.AddModelError("NewUsername", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(vm);
            }
            user.Username = newUsername;
            changedSomething = true;
        }

        if (!string.IsNullOrWhiteSpace(vm.NewPassword))
        {
            user.PasswordHash = PasswordHasher.Hash(vm.NewPassword);
            changedSomething = true;
        }

        if (!changedSomething)
        {
            ModelState.AddModelError("", "Değişiklik girilmedi.");
            return View(vm);
        }

        await Db.SaveChangesAsync();
        TempData["Success"] = "Hesap bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }
}