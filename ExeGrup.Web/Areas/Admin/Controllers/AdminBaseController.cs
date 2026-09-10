using ExeGrup.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExeGrup.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class AdminBaseController : Controller
{
    protected readonly AppDbContext Db;

    protected AdminBaseController(AppDbContext db) => Db = db;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.UnreadMessages = Db.ContactMessages.Count(m => !m.IsRead);
        base.OnActionExecuting(context);
    }
}