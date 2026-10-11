// Zonta theme manager: dark/light mode with animated toggle.
// Preference order: saved choice (localStorage) -> OS prefers-color-scheme.
// An inline snippet in <head> pre-applies data-theme before first paint (no flash);
// this file wires the toggle button and keeps the icon in sync.
(function () {
    'use strict';

    var STORAGE_KEY = 'zonta-theme';

    function getPreferred() {
        try {
            var saved = window.localStorage.getItem(STORAGE_KEY);
            if (saved === 'dark' || saved === 'light') return saved;
        } catch (e) { /* storage unavailable: fall through */ }
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light';
    }

    function apply(theme, animate) {
        var root = document.documentElement;
        var next = theme === 'dark' ? 'dark' : 'light';
        if (animate !== false) {
            root.classList.add('theme-animating');
            window.setTimeout(function () { root.classList.remove('theme-animating'); }, 400);
        }
        root.setAttribute('data-theme', next);
        try { window.localStorage.setItem(STORAGE_KEY, next); } catch (e) { /* ignore */ }
        syncIcon(next, animate !== false);
        var btn = document.getElementById('themeToggle');
        if (btn) {
            var s = window.zontaStrings || {};
            btn.setAttribute('aria-pressed', next === 'dark' ? 'true' : 'false');
            btn.setAttribute('title', next === 'dark'
                ? (s.themeLight || 'Switch to light mode')
                : (s.themeDark || 'Switch to dark mode'));
        }
    }

    function syncIcon(theme, animate) {
        var icon = document.getElementById('themeToggleIcon');
        if (!icon) return;
        var showMoon = theme !== 'dark'; // dark mode shows the sun (tap to go light)
        var target = showMoon ? 'fa-moon' : 'fa-sun';
        if (animate !== false) {
            icon.classList.add('theme-icon-spin');
            window.setTimeout(function () {
                icon.classList.remove('fa-moon', 'fa-sun', 'theme-icon-spin');
                icon.classList.add(target);
            }, 160);
        } else {
            icon.classList.remove('fa-moon', 'fa-sun');
            icon.classList.add(target);
        }
    }

    function current() {
        return document.documentElement.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
    }

    function init() {
        // If the head snippet already set a theme, respect it; otherwise resolve now.
        if (!document.documentElement.getAttribute('data-theme')) {
            apply(getPreferred(), false);
        } else {
            syncIcon(current(), false);
        }
        var btn = document.getElementById('themeToggle');
        if (btn) {
            btn.addEventListener('click', function () {
                apply(current() === 'dark' ? 'light' : 'dark', true);
            });
        }
        if (window.matchMedia) {
            var mq = window.matchMedia('(prefers-color-scheme: dark)');
            var onChange = function (e) {
                try {
                    if (!window.localStorage.getItem(STORAGE_KEY)) {
                        apply(e.matches ? 'dark' : 'light', true);
                    }
                } catch (err) { /* ignore */ }
            };
            if (mq.addEventListener) mq.addEventListener('change', onChange);
            else if (mq.addListener) mq.addListener(onChange);
        }
    }

    window.ZontaTheme = { apply: apply, current: current, init: init };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
