using System.Net;
using ExeGrup.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ExeGrup.Web.Services;

public interface ISettingsService
{
    string Get(string key, string defaultValue = "");
    Task SetAsync(string key, string value);
    string WaLink();
}

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public SettingsService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public string Get(string key, string defaultValue = "")
    {
        return _cache.GetOrCreate($"setting:{key}", entry =>
        {
            entry.Priority = CacheItemPriority.NeverRemove;
            return _db.SiteSettings.AsNoTracking().FirstOrDefault(s => s.Key == key)?.Value ?? defaultValue;
        }) ?? defaultValue;
    }

    public async Task SetAsync(string key, string value)
    {
        var setting = await _db.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting is null)
            _db.SiteSettings.Add(new Models.SiteSetting { Key = key, Value = value });
        else
            setting.Value = value;

        await _db.SaveChangesAsync();
        _cache.Remove($"setting:{key}");
    }

    public string WaLink()
    {
        var baseLink = Get("WhatsAppLink", "https://wa.me/905405505255");
        var message = Get("WhatsAppMessage", "Merhaba, teklif almak istiyorum.");
        return $"{baseLink}?text={WebUtility.UrlEncode(message)}";
    }
}