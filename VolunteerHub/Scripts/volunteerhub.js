// VolunteerHub – client-side utilities
(function () {
    'use strict';

    // ── Active nav link highlighting ──────────────────────────
    function highlightNav() {
        var path = window.location.pathname.toLowerCase();
        document.querySelectorAll('.vh-nav-item').forEach(function (a) {
            var href = (a.getAttribute('href') || '').toLowerCase();
            if (href && path.indexOf(href.replace(/^.*\/pages\//, '').split('.')[0]) !== -1) {
                a.classList.add('active');
            }
        });
    }

    // ── Success banner auto-hide ──────────────────────────────
    function autoHideBanners() {
        var banners = document.querySelectorAll('.vh-alert-success[data-autohide]');
        banners.forEach(function (b) {
            setTimeout(function () {
                b.style.transition = 'opacity 0.5s';
                b.style.opacity = '0';
                setTimeout(function () { b.style.display = 'none'; }, 500);
            }, 4000);
        });
    }

    // ── Profile image preview ─────────────────────────────────
    function initImagePreview(inputId, previewId) {
        var input   = document.getElementById(inputId);
        var preview = document.getElementById(previewId);
        if (!input || !preview) return;
        input.addEventListener('change', function () {
            var file = input.files && input.files[0];
            if (file && file.type.startsWith('image/')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = 'block';
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // ── Confirm delete ────────────────────────────────────────
    function initConfirmLinks() {
        document.querySelectorAll('[data-confirm]').forEach(function (el) {
            el.addEventListener('click', function (e) {
                if (!confirm(el.getAttribute('data-confirm') || 'Are you sure?')) {
                    e.preventDefault();
                }
            });
        });
    }

    // ── Sidebar toggle (mobile) ───────────────────────────────
    function initSidebarToggle() {
        var toggle  = document.getElementById('vh-sidebar-toggle');
        var sidebar = document.querySelector('.vh-sidebar');
        if (!toggle || !sidebar) return;
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });
    }

    // Global helpers called by onclick attributes in DashboardMaster
    window.toggleSidebar = function () {
        var sidebar = document.getElementById('vhSidebar');
        var overlay = document.getElementById('sidebarOverlay');
        if (!sidebar) return;
        var isOpen = sidebar.classList.toggle('open');
        if (overlay) overlay.classList.toggle('show', isOpen);
    };

    window.closeSidebar = function () {
        var sidebar = document.getElementById('vhSidebar');
        var overlay = document.getElementById('sidebarOverlay');
        if (sidebar) sidebar.classList.remove('open');
        if (overlay) overlay.classList.remove('show');
    };

    // ── Number counter animation for stat cards ───────────────
    function animateCounters() {
        document.querySelectorAll('[data-count]').forEach(function (el) {
            var target = parseFloat(el.getAttribute('data-count')) || 0;
            var start  = 0;
            var step   = target / 30;
            var isFloat = el.getAttribute('data-float') === '1';
            var timer  = setInterval(function () {
                start += step;
                if (start >= target) {
                    start = target;
                    clearInterval(timer);
                }
                el.textContent = isFloat ? start.toFixed(1) : Math.round(start).toLocaleString();
            }, 20);
        });
    }

    // ── Init all ──────────────────────────────────────────────
    document.addEventListener('DOMContentLoaded', function () {
        highlightNav();
        autoHideBanners();
        initConfirmLinks();
        initSidebarToggle();
        animateCounters();
        // image preview inputs are initialised per-page via VH.initImagePreview()

        // Photo thumbnail click → gallery lightbox
        document.addEventListener('click', function (e) {
            var thumb = e.target.closest('.vh-photo-thumb');
            if (!thumb) return;
            try {
                var images = JSON.parse(thumb.getAttribute('data-images'));
                VH.showGallery(images);
            } catch (ex) { /* ignore parse errors */ }
        });
    });

    // Expose public API
    window.VH = {
        initImagePreview: initImagePreview,
        // ── Photo gallery lightbox ────────────────────────────────
        showGallery: function (images) {
            if (!images || !images.length) return;
            // Create overlay
            var overlay = document.createElement('div');
            overlay.className = 'vh-gallery-overlay';
            overlay.onclick = function (e) { if (e.target === overlay) overlay.remove(); };

            // Close button
            var close = document.createElement('button');
            close.className = 'vh-gallery-close';
            close.innerHTML = '&times;';
            close.onclick = function () { overlay.remove(); };
            overlay.appendChild(close);

            // Image container
            var wrap = document.createElement('div');
            wrap.className = 'vh-gallery-wrap';
            images.forEach(function (src) {
                var img = document.createElement('img');
                img.src = src;
                img.className = 'vh-gallery-img';
                wrap.appendChild(img);
            });
            overlay.appendChild(wrap);
            document.body.appendChild(overlay);

            // Close on Escape key
            var escHandler = function (e) {
                if (e.key === 'Escape') { overlay.remove(); document.removeEventListener('keydown', escHandler); }
            };
            document.addEventListener('keydown', escHandler);
        },
        // Build a Chart.js bar chart
        barChart: function (canvasId, labels, data, color) {
            var ctx = document.getElementById(canvasId);
            if (!ctx) return;
            color = color || '#4F46E5';
            var hexToRgba = function (hex, a) {
                var r = parseInt(hex.slice(1, 3), 16);
                var g = parseInt(hex.slice(3, 5), 16);
                var b = parseInt(hex.slice(5, 7), 16);
                return 'rgba(' + r + ',' + g + ',' + b + ',' + a + ')';
            };
            return new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: hexToRgba(color, 0.2),
                        borderColor: color,
                        borderWidth: 2,
                        borderRadius: 6,
                        borderSkipped: false
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: 'rgba(0,0,0,0.04)' },
                            ticks: { color: '#94A3B8', font: { size: 12 } }
                        },
                        x: {
                            grid: { display: false },
                            ticks: { color: '#94A3B8', font: { size: 12 } }
                        }
                    }
                }
            });
        },
        // Build a Chart.js line chart
        lineChart: function (canvasId, labels, data, color) {
            var ctx = document.getElementById(canvasId);
            if (!ctx) return;
            color = color || '#10B981';
            return new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        borderColor: color,
                        backgroundColor: color.replace(')', ',0.08)').replace('rgb', 'rgba'),
                        borderWidth: 2.5,
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: color,
                        pointRadius: 4,
                        pointHoverRadius: 6
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: {
                            beginAtZero: true,
                            grid: { color: 'rgba(0,0,0,0.04)' },
                            ticks: { color: '#94A3B8', font: { size: 12 } }
                        },
                        x: {
                            grid: { display: false },
                            ticks: { color: '#94A3B8', font: { size: 12 } }
                        }
                    }
                }
            });
        }
    };
}());
