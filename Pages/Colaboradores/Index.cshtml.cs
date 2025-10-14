
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services; // Adicione o using para o ApiClient
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Colaboradores
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        // Corre��o 1: Injetar o ApiClient diretamente, assim como nas outras p�ginas.
        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Corre��o 2: Usar o ColaboradorViewModel, que � o tipo correto retornado pela API.
        public IList<ColaboradorViewModel> Colaboradores { get; set; } = new List<ColaboradorViewModel>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public async Task OnGetAsync()
        {
            var colaboradores = await _apiClient.GetColaboradoresAsync(SearchString);
            if (colaboradores != null)
            {
                Colaboradores = colaboradores;
            }
        }
    }
}
