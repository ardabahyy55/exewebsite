using System.Text;
using ExeGrup.Web.Data;
using ExeGrup.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Controllers;

public class SeoController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISettingsService _settings;

    public SeoController(AppDbContext db, ISettingsService settings)
    {
        _db = db;
        _settings = settings;
    }

    [HttpGet("sitemap.xml")]
    public async Task<ContentResult> Sitemap()
    {
        var baseUrl = _settings.Get("SiteUrl", "").TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
            baseUrl = $"{Request.Scheme}://{Request.Host}";

        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var urls = new List<(string loc, string? lastmod)>
        {
            ("/", today), ("/hizmetler", today), ("/projeler", today),
            ("/galeri", today), ("/hakkimizda", today), ("/iletisim", today)
        };

        foreach (var slug in await _db.Services.AsNoTracking().Where(s => s.IsActive).Select(s => s.Slug).ToListAsync())
            urls.Add(("/" + slug, today));

        foreach (var slug in await _db.Projects.AsNoTracking().Where(p => p.IsActive).Select(p => p.Slug).ToListAsync())
            urls.Add(("/projeler/" + slug, today));

        var sb = new StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        foreach (var (loc, lastmod) in urls)
        {
            sb.Append("<url><loc>").Append(XmlEscape(baseUrl + loc)).Append("</loc>");
            if (lastmod != null) sb.Append("<lastmod>").Append(lastmod).Append("</lastmod>");
            sb.Append("</url>");
        }
        sb.Append("</urlset>");

        return Content(sb.ToString(), "application/xml; charset=utf-8");
    }

    [HttpGet("robots.txt")]
    public ContentResult Robots()
    {
        var baseUrl = _settings.Get("SiteUrl", "").TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
            baseUrl = $"{Request.Scheme}://{Request.Host}";

        var content =
            "User-agent: *\n" +
            "Allow: /\n" +
            "Disallow: /ardabahaadmin\n\n" +
            $"Sitemap: {baseUrl}/sitemap.xml\n";

        return Content(content, "text/plain; charset=utf-8");
    }

    private static string XmlEscape(string s) => s
        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        .Replace("\"", "&quot;").Replace("'", "&apos;");
}