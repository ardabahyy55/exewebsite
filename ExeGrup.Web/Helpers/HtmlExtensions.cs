using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExeGrup.Web.Helpers;

public static class HtmlExtensions
{
    // XSS güvenli çok satırlı metin: önce encode, sonra satır sonu -> <br />
    public static IHtmlContent Multiline(this IHtmlHelper helper, string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return HtmlString.Empty;
        var encoded = helper.Encode(text);
        return new HtmlString(encoded.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />"));
    }
}