using System.ComponentModel.DataAnnotations;

namespace ExeGrup.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Şifre gereklidir.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Ad soyad gereklidir."), StringLength(150)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Telefon gereklidir."), StringLength(30)]
    [RegularExpression("^[0-9+()\\-\\s]{7,20}$", ErrorMessage = "Geçerli bir telefon giriniz.")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "E-posta gereklidir."), StringLength(200), EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [StringLength(200)]
    [Display(Name = "Firma")]
    public string? Company { get; set; }

    [StringLength(300)]
    [Display(Name = "Konu")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Mesaj gereklidir."), StringLength(4000)]
    [Display(Name = "Mesaj")]
    public string Message { get; set; } = "";
}

public class ProjectFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Proje adı gereklidir."), StringLength(200)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Slug gereklidir."), StringLength(220)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug yalnızca küçük harf, rakam ve tire içerebilir.")]
    public string Slug { get; set; } = "";

    [Required(ErrorMessage = "Kategori gereklidir."), StringLength(100)]
    public string Category { get; set; } = "";

    [Required(ErrorMessage = "Kısa açıklama gereklidir."), StringLength(500)]
    public string ShortDescription { get; set; } = "";

    [Required(ErrorMessage = "Açıklama gereklidir.")]
    public string Description { get; set; } = "";

    public string? CoverImage { get; set; }
    public IFormFile? CoverImageFile { get; set; }

    [StringLength(200)]
    public string? Material { get; set; }

    [StringLength(200)]
    public string? ApplicationType { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
    public bool IsActive { get; set; } = true;

    public List<IFormFile> ImageFiles { get; set; } = new();
    public List<ExeGrup.Web.Models.ProjectImage> Images { get; set; } = new();
}

public class ServiceFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık gereklidir."), StringLength(150)]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Slug gereklidir."), StringLength(170)]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug yalnızca küçük harf, rakam ve tire içerebilir.")]
    public string Slug { get; set; } = "";

    [Required(ErrorMessage = "Kısa açıklama gereklidir."), StringLength(400)]
    public string ShortDescription { get; set; } = "";

    [Required(ErrorMessage = "Detaylı açıklama gereklidir.")]
    public string Description { get; set; } = "";

    public string? Image { get; set; }
    public IFormFile? ImageFile { get; set; }

    [StringLength(200)]
    public string? SeoTitle { get; set; }

    [StringLength(300)]
    public string? SeoDescription { get; set; }

    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SettingsFormViewModel
{
    public string CompanyName { get; set; } = "";
    public string CompanySubBrand { get; set; } = "";
    public string FooterText { get; set; } = "";
    public string PhoneDisplay { get; set; } = "";
    public string PhoneLink { get; set; } = "";
    public string WhatsAppLink { get; set; } = "";
    public string WhatsAppMessage { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string Instagram { get; set; } = "";
    public string MapsEmbed { get; set; } = "";
    public string SiteUrl { get; set; } = "";
    public string SeoTitle { get; set; } = "";
    public string SeoDescription { get; set; } = "";
    public IFormFile? LogoFile { get; set; }
    public IFormFile? FaviconFile { get; set; }
}

public class ChangeAccountViewModel
{
    [Required(ErrorMessage = "Mevcut şifre gereklidir."), DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Yeni kullanıcı adı")]
    public string? NewUsername { get; set; }

    [DataType(DataType.Password), StringLength(100, MinimumLength = 8, ErrorMessage = "Yeni şifre en az 8 karakter olmalıdır.")]
    [Display(Name = "Yeni şifre")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password), Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Yeni şifre (tekrar)")]
    public string? NewPasswordConfirm { get; set; }
}