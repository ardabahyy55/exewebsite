(function () {
    "use strict";

    // ---- Header: scroll durumu ----
    var header = document.getElementById("siteHeader");
    function updateHeader() {
        if (!header) return;
        if (header.classList.contains("header-transparent")) {
            header.classList.toggle("scrolled", window.scrollY > 40);
        }
    }
    window.addEventListener("scroll", updateHeader, { passive: true });
    updateHeader();

    // ---- Mobil menü ----
    var toggle = document.getElementById("navToggle");
    var nav = document.getElementById("mainNav");
    if (toggle && nav) {
        toggle.addEventListener("click", function () {
            var open = nav.classList.toggle("open");
            toggle.setAttribute("aria-expanded", open ? "true" : "false");
        });
        // Menü dışına tıklayınca kapat
        document.addEventListener("click", function (e) {
            if (nav.classList.contains("open") && !nav.contains(e.target) && !toggle.contains(e.target)) {
                nav.classList.remove("open");
                toggle.setAttribute("aria-expanded", "false");
            }
        });
    }

    // ---- Galeri filtresi ----
    var filterBtns = document.querySelectorAll(".filter-btn");
    var galleryItems = document.querySelectorAll("#galleryGrid .gallery-item");
    filterBtns.forEach(function (btn) {
        btn.addEventListener("click", function () {
            filterBtns.forEach(function (b) { b.classList.remove("active"); });
            btn.classList.add("active");
            var filter = btn.getAttribute("data-filter");
            galleryItems.forEach(function (item) {
                var show = filter === "all" || item.getAttribute("data-category") === filter;
                item.style.display = show ? "" : "none";
            });
        });
    });

    // ---- Lightbox ----
    var lightbox = document.getElementById("lightbox");
    if (lightbox) {
        var lbImg = lightbox.querySelector("img");
        var visibleItems = function () {
            return Array.prototype.filter.call(document.querySelectorAll(".lightbox-trigger"), function (el) {
                return el.offsetParent !== null;
            });
        };
        var current = -1;

        function openLb(trigger) {
            var items = visibleItems();
            current = items.indexOf(trigger);
            showLb();
            lightbox.hidden = false;
            document.body.style.overflow = "hidden";
        }
        function showLb() {
            var items = visibleItems();
            if (current < 0) current = items.length - 1;
            if (current >= items.length) current = 0;
            var el = items[current];
            lbImg.src = el.getAttribute("data-src");
            lbImg.alt = el.getAttribute("data-alt") || "";
        }
        function closeLb() {
            lightbox.hidden = true;
            document.body.style.overflow = "";
            lbImg.src = "";
        }

        document.querySelectorAll(".lightbox-trigger").forEach(function (trigger) {
            trigger.addEventListener("click", function () { openLb(trigger); });
        });
        lightbox.querySelector(".lb-close").addEventListener("click", closeLb);
        lightbox.querySelector(".lb-prev").addEventListener("click", function () { current--; showLb(); });
        lightbox.querySelector(".lb-next").addEventListener("click", function () { current++; showLb(); });
        lightbox.addEventListener("click", function (e) { if (e.target === lightbox) closeLb(); });
        document.addEventListener("keydown", function (e) {
            if (lightbox.hidden) return;
            if (e.key === "Escape") closeLb();
            if (e.key === "ArrowLeft") { current--; showLb(); }
            if (e.key === "ArrowRight") { current++; showLb(); }
        });
    }

    // ---- Başarı mesajını otomatik gizle ----
    var toast = document.querySelector(".toast-success");
    if (toast) {
        setTimeout(function () {
            toast.style.transition = "opacity .5s ease";
            toast.style.opacity = "0";
            setTimeout(function () { toast.remove(); }, 520);
        }, 6000);
    }
})();