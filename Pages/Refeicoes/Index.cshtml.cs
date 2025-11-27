using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Refeicoes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IList<RefeicaoViewModel> Refeicoes { get; set; } = new List<RefeicaoViewModel>();

        // Propriedades de Filtro (Vinculadas via GET)
        [BindProperty(SupportsGet = true)]
        public DateTime? DataFiltro { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TipoRefeicaoFiltro { get; set; }

        // Lista para o Dropdown de Tipos
        public SelectList TiposRefeicaoSL { get; set; }

        public async Task OnGetAsync()
        {
            // Se não tiver data, pode definir padrão "Hoje" ou deixar vazio para trazer tudo (limitado pela API)
            // DataFiltro ??= DateTime.Today; 

            // Popula o Dropdown
            var tipos = new List<string> { "Café_da_Manhã", "Almoço", "Jantar", "Ceia" };
            TiposRefeicaoSL = new SelectList(tipos);

            // Busca na API com os filtros
            Refeicoes = await _apiClient.GetRefeicoesAsync(DataFiltro, TipoRefeicaoFiltro);
        }
    }
}