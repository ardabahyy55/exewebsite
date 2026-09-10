namespace ExeGrup.Web.Services;

public interface IFileUploadService
{
    Task<string?> SaveImageAsync(IFormFile? file, string subfolder = "media");
    void DeleteImage(string? webPath);
}

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _env;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public FileUploadService(IWebHostEnvironment env) => _env = env;

    public async Task<string?> SaveImageAsync(IFormFile? file, string subfolder = "media")
    {
        if (file is null || file.Length == 0) return null;

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("Dosya boyutu en fazla 5 MB olabilir.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Yalnızca .jpg, .jpeg, .png ve .webp dosyaları yüklenebilir.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var bytes = ms.ToArray();

        if (!HasValidImageSignature(bytes, ext))
            throw new InvalidOperationException("Dosya içeriği geçerli bir görsel değil.");

        // Güvenli rastgele dosya adı — kullanıcıdan gelen orijinal ad kullanılmaz
        var name = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..12]}{ext}";

        var dir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", subfolder);
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, name);
        var root = Path.GetFullPath(Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads"));
        if (!Path.GetFullPath(fullPath).StartsWith(root))
            throw new InvalidOperationException("Geçersiz dosya yolu.");

        await File.WriteAllBytesAsync(fullPath, bytes);
        return $"/uploads/{subfolder}/{name}";
    }

    public void DeleteImage(string? webPath)
    {
        if (string.IsNullOrWhiteSpace(webPath) || !webPath.StartsWith("/uploads/")) return;

        var root = Path.GetFullPath(Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads"));
        var full = Path.GetFullPath(Path.Combine(_env.WebRootPath ?? "wwwroot", webPath.TrimStart('/')));

        // Path traversal koruması
        if (!full.StartsWith(root)) return;
        if (File.Exists(full)) File.Delete(full);
    }

    // MIME spoofing'e karşı magic byte kontrolü
    private static bool HasValidImageSignature(byte[] bytes, string ext) => ext switch
    {
        ".jpg" or ".jpeg" => bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
        ".png" => bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47,
        ".webp" => bytes.Length > 12 &&
                   bytes[0] == (byte)'R' && bytes[1] == (byte)'I' && bytes[2] == (byte)'F' && bytes[3] == (byte)'F' &&
                   bytes[8] == (byte)'W' && bytes[9] == (byte)'E' && bytes[10] == (byte)'B' && bytes[11] == (byte)'P',
        _ => false
    };
}