using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Departamentos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IList<Departamento> Departamentos { get; set; } = new List<Departamento>();

        public async Task OnGetAsync()
        {
            // Usa o serviço centralizado que já injeta o Token JWT
            Departamentos = await _apiClient.GetDepartamentosAsync();
        }
    }
}