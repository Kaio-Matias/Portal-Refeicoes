document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('main-content');
    const sidebarToggleOpen = document.getElementById('sidebarToggleOpen'); // Botão mobile abrir
    const sidebarToggleClose = document.getElementById('sidebarToggleClose'); // Botão mobile fechar
    const sidebarToggleDesktop = document.getElementById('sidebarToggleDesktop'); // Botão desktop
    const body = document.body;
    const overlay = document.createElement('div'); // Cria o elemento overlay dinamicamente

    const SIDEBAR_STATE_KEY = 'sidebarCollapsedState';

    // Configura o overlay (para fechar a sidebar mobile ao clicar fora)
    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.right = '0';
    overlay.style.bottom = '0';
    overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
    overlay.style.zIndex = '1030'; // Abaixo da sidebar, acima do conteúdo
    overlay.style.display = 'none'; // Começa escondido
    overlay.id = 'sidebar-overlay'; // Para referência
    document.body.appendChild(overlay);

    // Função para alternar o estado no Desktop
    function toggleDesktopSidebar() {
        if (sidebar && mainContent && window.innerWidth >= 768) {
            sidebar.classList.toggle('collapsed');
            const isCollapsed = sidebar.classList.contains('collapsed');
            localStorage.setItem(SIDEBAR_STATE_KEY, isCollapsed);
            initializeOrDestroyTooltips(); // Atualiza os tooltips
        }
    }

    // Função para abrir/fechar no Mobile
    function toggleMobileSidebar(open) {
        if (sidebar && window.innerWidth < 768) {
            if (open) {
                sidebar.classList.add('active');
                overlay.style.display = 'block'; // Mostra o overlay
                // body.classList.add('sidebar-open'); // Classe no body não é mais necessária para o overlay
            } else {
                sidebar.classList.remove('active');
                overlay.style.display = 'none'; // Esconde o overlay
                // body.classList.remove('sidebar-open');
            }
        }
    }

    // --- Event Listeners ---

    // Botões Mobile
    if (sidebarToggleOpen) {
        sidebarToggleOpen.addEventListener('click', (e) => {
            e.stopPropagation(); // Previne fechar imediatamente
            toggleMobileSidebar(true);
        });
    }
    if (sidebarToggleClose) {
        sidebarToggleClose.addEventListener('click', () => toggleMobileSidebar(false));
    }

    // Botão Desktop
    if (sidebarToggleDesktop) {
        sidebarToggleDesktop.addEventListener('click', toggleDesktopSidebar);
    }

    // Fechar sidebar mobile ao clicar no overlay
    overlay.addEventListener('click', () => toggleMobileSidebar(false));

    // --- Inicialização ---

    // Aplica o estado salvo no Desktop ao carregar a página
    function applyInitialSidebarState() {
        const isDesktopCollapsed = localStorage.getItem(SIDEBAR_STATE_KEY) === 'true';
        if (window.innerWidth >= 768 && sidebar) { // Aplica só se for desktop
            if (isDesktopCollapsed) {
                sidebar.classList.add('collapsed');
            } else {
                sidebar.classList.remove('collapsed'); // Garante que não está collapsed se não deveria
            }
            initializeOrDestroyTooltips(); // Atualiza tooltips no carregamento
        } else if (sidebar) {
            // Em telas mobile, garante que a sidebar comece fechada e sem a classe collapsed
            sidebar.classList.remove('active', 'collapsed');
            overlay.style.display = 'none';
            initializeOrDestroyTooltips(); // Destroi tooltips se houver
        }
    }


    // Inicializa Tooltips do Bootstrap (para os ícones quando recolhido no desktop)
    let activeTooltips = []; // Guarda referência aos tooltips ativos
    function initializeOrDestroyTooltips() {
        // Destroi tooltips antigos
        activeTooltips.forEach(tooltip => tooltip.dispose());
        activeTooltips = [];

        if (sidebar) {
            const isCollapsed = sidebar.classList.contains('collapsed');
            const tooltipTriggerList = [].slice.call(sidebar.querySelectorAll('.nav-link[title]'));

            // Cria novos tooltips apenas se estiver recolhido e em tela grande
            if (isCollapsed && window.innerWidth >= 768) {
                activeTooltips = tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl, {
                        placement: 'right',
                        trigger: 'hover',
                        boundary: document.body // Previne problemas de posicionamento
                    });
                });
            }
        }
    }

    // Aplica estado inicial ao carregar
    applyInitialSidebarState();

    // Reavalia o estado da sidebar ao redimensionar a janela
    window.addEventListener('resize', () => {
        applyInitialSidebarState(); // Reaplica estado e tooltips corretos para o novo tamanho
        // Se a sidebar mobile estava aberta e a tela ficou grande, fecha a mobile
        if (window.innerWidth >= 768 && sidebar && sidebar.classList.contains('active')) {
            toggleMobileSidebar(false);
        }
    });

});