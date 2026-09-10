using Microsoft.AspNetCore.Mvc;

namespace ExeGrup.Web.Controllers;

public class ErrorsController : Controller
{
    [HttpGet("hata/{code:int?}")]
    public IActionResult Status(int? code)
    {
        var c = code ?? 500;
        if (c != 404 && c != 500) c = 500;
        Response.StatusCode = c;
        ViewData["Title"] = c == 404 ? "Sayfa Bulunamadı | tabela.exe" : "Hata | tabela.exe";
        return View("Status", c);
    }
}