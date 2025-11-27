document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggleOpen = document.getElementById('sidebarToggleOpen');
    const sidebarToggleClose = document.getElementById('sidebarToggleClose');
    const sidebarToggleDesktop = document.getElementById('sidebarToggleDesktop');

    // Criação do overlay dinâmico para mobile
    let overlay = document.querySelector('.sidebar-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'sidebar-overlay';
        // Estilos básicos do overlay caso não estejam no CSS
        overlay.style.position = 'fixed';
        overlay.style.top = '0';
        overlay.style.left = '0';
        overlay.style.width = '100%';
        overlay.style.height = '100%';
        overlay.style.backgroundColor = 'rgba(0,0,0,0.5)';
        overlay.style.zIndex = '1030';
        overlay.style.display = 'none';
        document.body.appendChild(overlay);
    }

    const SIDEBAR_STATE_KEY = 'sidebarCollapsedState';

    // --- Funções ---

    function toggleDesktop() {
        if (window.innerWidth >= 768) {
            sidebar.classList.toggle('collapsed');
            const isCollapsed = sidebar.classList.contains('collapsed');
            localStorage.setItem(SIDEBAR_STATE_KEY, isCollapsed);

            // Ajusta ícone do botão (opcional, se usar font-awesome)
            const icon = sidebarToggleDesktop.querySelector('i');
            if (icon) {
                // A rotação já é tratada via CSS, mas se quiser trocar a classe:
                // icon.classList.toggle('fa-chevron-left');
                // icon.classList.toggle('fa-chevron-right');
            }
        }
    }

    function toggleMobile(show) {
        if (show) {
            sidebar.classList.add('active');
            overlay.style.display = 'block';
            document.body.style.overflow = 'hidden'; // Previne scroll do body
        } else {
            sidebar.classList.remove('active');
            overlay.style.display = 'none';
            document.body.style.overflow = '';
        }
    }

    // --- Event Listeners ---

    if (sidebarToggleDesktop) {
        sidebarToggleDesktop.addEventListener('click', toggleDesktop);
    }

    if (sidebarToggleOpen) {
        sidebarToggleOpen.addEventListener('click', (e) => {
            e.stopPropagation();
            toggleMobile(true);
        });
    }

    if (sidebarToggleClose) {
        sidebarToggleClose.addEventListener('click', () => toggleMobile(false));
    }

    if (overlay) {
        overlay.addEventListener('click', () => toggleMobile(false));
    }

    // --- Inicialização ---

    function init() {
        // Recupera estado salvo apenas no Desktop
        if (window.innerWidth >= 768) {
            const savedState = localStorage.getItem(SIDEBAR_STATE_KEY);
            if (savedState === 'true') {
                sidebar.classList.add('collapsed');
            } else {
                sidebar.classList.remove('collapsed');
            }
        } else {
            // Mobile começa sempre fechado e expandido (estrutura interna)
            sidebar.classList.remove('collapsed');
            sidebar.classList.remove('active');
        }

        // Inicializar Tooltips do Bootstrap se necessário
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        })
    }

    init();

    // Ajuste ao redimensionar tela
    window.addEventListener('resize', () => {
        if (window.innerWidth >= 768) {
            // Se cresceu a tela, remove o estado mobile
            overlay.style.display = 'none';
            sidebar.classList.remove('active');
            document.body.style.overflow = '';

            // Recupera estado de colapso
            const savedState = localStorage.getItem(SIDEBAR_STATE_KEY);
            if (savedState === 'true') sidebar.classList.add('collapsed');
        } else {
            // Se diminuiu, remove colapso (mobile usa menu cheio)
            sidebar.classList.remove('collapsed');
        }
    });
});