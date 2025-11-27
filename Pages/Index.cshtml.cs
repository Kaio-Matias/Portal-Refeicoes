using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Portal_Refeicoes.Pages
{
    [Authorize] // Apenas usuários logados podem ver o Dashboard
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public DashboardStatsViewModel Stats { get; set; } = new();
        public List<RegistroRecenteViewModel> RegistrosRecentes { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Busca contadores
            Stats = await _apiClient.GetDashboardStatsAsync();

            // Busca lista recente (A API filtra pelo dia atual automaticamente)
            RegistrosRecentes = await _apiClient.GetRegistrosRecentesAsync();
        }
    }
}