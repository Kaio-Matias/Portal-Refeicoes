using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages
{
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
            Stats = await _apiClient.GetDashboardStatsAsync();
            RegistrosRecentes = await _apiClient.GetRegistrosRecentesAsync();
        }
    }
}