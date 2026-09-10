using ExeGrup.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("ardabahaadmin/mesajlar")]
public class MessagesController : AdminBaseController
{
    public MessagesController(AppDbContext db) : base(db) { }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? durum)
    {
        var query = Db.ContactMessages.AsNoTracking();
        if (durum == "okunmamis") query = query.Where(m => !m.IsRead);

        ViewBag.Filter = durum;
        return View(await query.OrderByDescending(m => m.CreatedAt).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var message = await Db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();

        if (!message.IsRead)
        {
            message.IsRead = true;
            await Db.SaveChangesAsync();
        }
        return View(message);
    }

    [HttpPost("{id:int}/okundu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleRead(int id)
    {
        var message = await Db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();
        message.IsRead = !message.IsRead;
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("sil/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await Db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();
        Db.ContactMessages.Remove(message);
        await Db.SaveChangesAsync();
        TempData["Success"] = "Mesaj silindi.";
        return RedirectToAction(nameof(Index));
    }
}