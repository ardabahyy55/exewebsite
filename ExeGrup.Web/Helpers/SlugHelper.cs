using System.Text;
using System.Text.RegularExpressions;

namespace ExeGrup.Web.Helpers;

public static class SlugHelper
{
    private static readonly Dictionary<char, string> Map = new()
    {
        ['ç'] = "c", ['Ç'] = "c", ['ğ'] = "g", ['Ğ'] = "g", ['ı'] = "i", ['I'] = "i", ['İ'] = "i",
        ['ö'] = "o", ['Ö'] = "o", ['ş'] = "s", ['Ş'] = "s", ['ü'] = "u", ['Ü'] = "u"
    };

    public static string Slugify(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        var sb = new StringBuilder();
        foreach (var raw in input.Trim())
        {
            if (Map.TryGetValue(raw, out var mapped)) { sb.Append(mapped); continue; }

            var c = char.ToLowerInvariant(raw);
            if (Map.TryGetValue(c, out mapped)) { sb.Append(mapped); continue; }

            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) sb.Append(c);
            else sb.Append('-');
        }

        return Regex.Replace(sb.ToString(), "-{2,}", "-").Trim('-');
    }
}