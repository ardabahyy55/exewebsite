(function () {
    "use strict";

    // ---- Admin mobil menü ----
    var toggle = document.getElementById("adminToggle");
    var sidebar = document.getElementById("adminSidebar");
    if (toggle && sidebar) {
        toggle.addEventListener("click", function () { sidebar.classList.toggle("open"); });
        document.addEventListener("click", function (e) {
            if (sidebar.classList.contains("open") && !sidebar.contains(e.target) && !toggle.contains(e.target)) {
                sidebar.classList.remove("open");
            }
        });
    }

    // ---- Başlıktan otomatik slug ----
    function slugify(input) {
        var map = { "ç": "c", "Ç": "c", "ğ": "g", "Ğ": "g", "ı": "i", "İ": "i", "ö": "o", "Ö": "o", "ş": "s", "Ş": "s", "ü": "u", "Ü": "u" };
        var out = "";
        for (var ch of input.trim()) {
            if (map[ch]) out += map[ch];
            else {
                var low = ch.toLowerCase();
                if (/[a-z0-9]/.test(low)) out += low;
                else out += "-";
            }
        }
        return out.replace(/-{2,}/g, "-").replace(/^-+|-+$/g, "");
    }

    document.querySelectorAll("input[data-slug-target]").forEach(function (source) {
        var target = document.getElementById(source.getAttribute("data-slug-target"));
        if (!target) return;
        source.addEventListener("input", function () {
            if (target.dataset.touched === "1") return;
            target.value = slugify(source.value);
        });
        source.addEventListener("change", function () {
            if (!target.value) target.value = slugify(source.value);
        });
        target.addEventListener("input", function () { target.dataset.touched = "1"; });
    });
})();