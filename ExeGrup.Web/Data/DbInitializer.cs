using ExeGrup.Web.Models;
using ExeGrup.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext db, IConfiguration config)
    {
        db.Database.Migrate();
        SeedAsync(db, config).GetAwaiter().GetResult();
    }

    private static async Task SeedAsync(AppDbContext db, IConfiguration config)
    {
        // ---- İlk admin hesabı (şifre appsettings / environment variable'dan) ----
        var username = config["Admin:Username"] ?? "exegrupadmin";
        var password = config["Admin:Password"];

        if (!db.AdminUsers.Any() && !string.IsNullOrWhiteSpace(password))
        {
            db.AdminUsers.Add(new AdminUser
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                CreatedAt = DateTime.Now
            });
        }

        // ---- Varsayılan site ayarları ----
        foreach (var (key, value) in DefaultSettings)
        {
            if (!db.SiteSettings.Any(s => s.Key == key))
                db.SiteSettings.Add(new SiteSetting { Key = key, Value = value });
        }

        // ---- Varsayılan hizmetler ----
        if (!db.Services.Any())
            db.Services.AddRange(DefaultServices());

        await db.SaveChangesAsync();
    }

    public static readonly Dictionary<string, string> DefaultSettings = new()
    {
        ["CompanyName"] = "EXE GRUP",
        ["CompanySubBrand"] = "tabela.exe",
        ["FooterText"] = "Dış Mekan Tabela ve Reklam Çözümleri",
        ["PhoneDisplay"] = "0540 550 52 55",
        ["PhoneLink"] = "+905405505255",
        ["WhatsAppLink"] = "https://wa.me/905405505255",
        ["WhatsAppMessage"] = "Merhaba, tabela hizmetleriniz hakkında bilgi ve fiyat teklifi almak istiyorum.",
        ["Email"] = "info@exegrup.com.tr",
        ["Address"] = "Çobanözü Mah. 9003 Cad. Toybelen, Küçük Sanayi Sitesi\n54. Blok No:2 D:3\n55200 Atakum / Samsun",
        ["Instagram"] = "https://www.instagram.com/tabela.exe/",
        ["MapsEmbed"] = "https://maps.google.com/maps?q=K%C3%BC%C3%A7%C3%BCk%20Sanayi%20Sitesi%20Toybelen%20Atakum%20Samsun&t=&z=14&ie=UTF8&iwloc=&output=embed",
        ["SiteUrl"] = "https://www.exegrup.com.tr",
        ["SeoTitle"] = "EXE GRUP | tabela.exe — Samsun Dış Mekan Tabela ve Kutu Harf",
        ["SeoDescription"] = "Samsun ve Atakum'da dış mekan tabela, kompozit kutu harf, ışıklı tabela ve kurumsal cephe tabela üretimi. Keşif, tasarım, üretim ve montaj tek elden.",
        ["LogoPath"] = "",
        ["FaviconPath"] = ""
    };

    private static List<Service> DefaultServices() => new()
    {
        new Service
        {
            Title = "Dış Mekan Tabela", Slug = "dis-mekan-tabela", SortOrder = 1,
            ShortDescription = "Cephenize değer katan, uzaktan dikkat çeken dış mekan tabela çözümleri: tasarım, üretim ve montaj tek elden.",
            Description = "Dış mekan tabela, işletmenizin görünürlüğünü doğrudan etkileyen en önemli reklam yatırımlarından biridir. tabela.exe olarak Samsun ve çevresinde; kompozit, pleksi ve metal malzemelerle işletmenizin mimarisine uygun dış mekan tabela tasarlıyor, üretiyor ve monte ediyoruz.\n\nKeşif ve ölçü almadan montaj teslimine kadar tüm süreci tek elden yönetir; malzeme seçiminden aydınlatma detaylarına kadar her aşamada size net bilgiler sunarız. Mağaza, plaza, fabrika, kafe, restoran ve ofis gibi farklı ölçeklerdeki işletmeler için cepheye uygun çözümler üretiriz.",
            SeoTitle = "Samsun Dış Mekan Tabela | tabela.exe — EXE GRUP",
            SeoDescription = "Samsun ve Atakum'da dış mekan tabela üretimi: kompozit, ışıklı ve kutu harf cephe tabelaları. Keşif, üretim ve montaj tek elden. Teklif için: 0540 550 52 55."
        },
        new Service
        {
            Title = "Kompozit Kutu Harf", Slug = "kompozit-kutu-harf", SortOrder = 2,
            ShortDescription = "Kompozit yüzeyli kutu harflerle modern, dayanıklı ve keskin hatlı tabela uygulamaları.",
            Description = "Kompozit kutu harf; alüminyum kompozit panelin CNC ile işlenip harf formuna dönüştürülmesiyle elde edilen, düz ve keskin yüzeylere sahip modern bir tabela türüdür. Güneş, yağmur ve rüzgâra karşı dayanıklı yapısı sayesinde dış cephelerde yıllarca ilk günkü görünümünü korur.\n\nRenk ve doku seçenekleriyle markanıza uygun tasarım yapılır. Gündüz şık bir cephe elemanı, aydınlatmalı versiyonlarında ise gece etkili bir reklam aracı olarak çalışır. Samsun'daki işletmeler için kompozit kutu harf üretiminde ölçü, montaj ve aydınlatma detaylarını tek elden planlıyoruz.",
            SeoTitle = "Samsun Kompozit Kutu Harf | tabela.exe — EXE GRUP",
            SeoDescription = "Samsun ve Atakum'da kompozit kutu harf üretimi. CNC kesim, dayanıklı kompozit yüzey, opsiyonel LED aydınlatma ve yerinde montaj."
        },
        new Service
        {
            Title = "Işıklı Tabela", Slug = "isikli-tabela", SortOrder = 3,
            ShortDescription = "Gün batımından sonra da çalışan tabelalar: enerji verimli, uzun ömürlü LED aydınlatmalı tabela çözümleri.",
            Description = "Işıklı tabela, işletmenizin kapalı olduğu saatlerde de görünürlüğünü korumasını sağlar. LED aydınlatma teknolojisiyle üretilen ışıklı tabelalar; düşük enerji tüketimi ve uzun ömürlü yapısıyla tabelanızı gece de güçlü bir reklam aracına dönüştürür.\n\nIşıklı kutu harf, aydınlatmalı kompozit tabela ve ışıklı cephe uygulamaları başlıklarında; cephe ölçüsüne, çevre aydınlatmasına ve bütçenize uygun aydınlatma senaryosunu sizinle birlikte belirleriz. Samsun ve çevresinde ışıklı tabela üretimi ve montajı yapıyoruz.",
            SeoTitle = "Samsun Işıklı Tabela | tabela.exe — EXE GRUP",
            SeoDescription = "Samsun'da ışıklı tabela ve ışıklı kutu harf üretimi. LED aydınlatmalı, enerji verimli cephe tabelaları. Keşif ve montaj dahil."
        },
        new Service
        {
            Title = "Kutu Harf", Slug = "kutu-harf", SortOrder = 4,
            ShortDescription = "Pleksi ve kompozit yüzeyli, düz veya aydınlatmalı kutu harf üretimi.",
            Description = "Kutu harf; harflerin hafif bir gövde içinde pleksi ya da kompozit yüzey ile kaplandığı, logo ve marka adlarını keskin biçimde öne çıkaran bir tabela sistemidir. Yüzey seçimine göre gündüz mat ve derin bir görünüm, gece ise homojen bir ışık dağılımı sağlar.\n\nKutu harf üretiminde lazer kesim ve CNC teknolojilerini kullanır; harf derinliği, yüzey rengi ve aydınlatma tipini markanıza göre belirleriz. Mağaza girişlerinden fabrika cephelerine kadar farklı ölçeklerde kutu harf uygulamaları gerçekleştiriyoruz.",
            SeoTitle = "Samsun Kutu Harf | tabela.exe — EXE GRUP",
            SeoDescription = "Samsun ve Atakum'da kutu harf üretimi: pleksi ve kompozit yüzeyli, düz veya LED aydınlatmalı kutu harf uygulamaları."
        },
        new Service
        {
            Title = "Kurumsal Tabela", Slug = "kurumsal-tabela", SortOrder = 5,
            ShortDescription = "Plaza, ofis, fabrika ve kurumlar için tutarlı kurumsal kimliğe uygun tabela sistemleri.",
            Description = "Kurumsal tabela; binanın girişinden iç mekâna kadar marka kimliğinin tutarlı biçimde hissedilmesini sağlayan tabela sistemlerinin bütünüdür. Logo tabela, yönlendirme tabelaları, oda ve departman isimleri, fabrika güvenlik ve bilgilendirme tabelaları bu kapsamda yer alır.\n\nKurumunuzun kimliğine uygun malzeme ve tipografi seçimi yapıp tüm tabela ihtiyacını tek bir plan içinde ele alırız. Böylece dağınık değil, tek elden yönetilen bir kurumsal görüntü elde edilir.",
            SeoTitle = "Kurumsal Tabela | Samsun tabela firması — tabela.exe",
            SeoDescription = "Samsun'da kurumsal tabela: plaza, ofis, fabrika ve kurumlar için logo, yönlendirme ve bilgilendirme tabela sistemleri."
        },
        new Service
        {
            Title = "Cephe Tabela", Slug = "cephe-tabela", SortOrder = 6,
            ShortDescription = "Binanın mimarisiyle uyumlu, cepheye entegre tabela ve dış cephe reklam uygulamaları.",
            Description = "Cephe tabela, binanın ölçüsüne, cephe malzemesine ve mimari karakterine göre planlanan tabela uygulamalarıdır. Doğru planlanmış bir cephe tabelası hem uzak mesafeden okunabilirlik sağlar hem de binanın genel görünümüne katkı yapar.\n\nCephe ölçüsü alımı, taşıyıcı sistem detayları, montaj güvenliği ve bakım erişimi dahil olmak üzere tüm teknik detayları projelendirir; kompozit kaplama, kutu harf ve ışıklı tabela kombinasyonlarıyla bütüncül cephe çözümleri üretiriz.",
            SeoTitle = "Samsun Cephe Tabela | tabela.exe — EXE GRUP",
            SeoDescription = "Samsun ve çevresinde cephe tabela ve dış cephe reklam uygulamaları: kompozit kaplama, kutu harf ve ışıklı cephe sistemleri."
        },
        new Service
        {
            Title = "Mağaza & İşletme Tabelaları", Slug = "magaza-isletme-tabelalari", SortOrder = 7,
            ShortDescription = "Mağaza, kafe, restoran, kuaför ve her ölçekten işletme için doğru ölçekte tabela çözümleri.",
            Description = "Küçük bir dükkân tabelasından çok katlı bir mağaza cephesine kadar her işletmenin ihtiyacı farklıdır. tabela.exe olarak işletmenizin konumunu, hedef kitlesini ve bütçesini değerlendirir; en doğru tabela türünü ve ölçeğini birlikte belirleriz.\n\nVitrin üzeri uygulamalar, giriş tabelaları, menü ve ürün yönlendirme tabelaları gibi detayları da plana dahil eder; işletmenizin tamamını kapsayan tek bir tabela planı sunarız.",
            SeoTitle = "Mağaza ve İşletme Tabelaları | Samsun tabela — tabela.exe",
            SeoDescription = "Samsun'da mağaza ve işletme tabelaları: dükkân tabelası, vitrin uygulamaları ve ışıklı tabela çözümleri. Teklif alın."
        }
    };
}