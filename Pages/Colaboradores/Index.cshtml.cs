using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Colaboradores
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IList<ColaboradorViewModel> Colaboradores { get; set; } = new List<ColaboradorViewModel>();

        // Propriedades de Filtro
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FiltroDepartamentoId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FiltroFuncaoId { get; set; }

        // Nova propriedade para controlar a flag "Todos"
        [BindProperty(SupportsGet = true)]
        public bool ExibirTodos { get; set; }

        // Propriedade para controlar o estado da tela (se buscou ou não)
        public bool BuscaRealizada { get; set; } = false;

        public SelectList DepartamentosSL { get; set; }
        public SelectList FuncoesSL { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Sempre carrega os dropdowns para o formulário funcionar
            var departamentos = await _apiClient.GetDepartamentosAsync();
            var funcoes = await _apiClient.GetFuncoesAsync();

            DepartamentosSL = new SelectList(departamentos, "Id", "Nome");
            FuncoesSL = new SelectList(funcoes, "Id", "Nome");

            // 2. Verifica se o usuário aplicou algum filtro
            bool temFiltro = !string.IsNullOrEmpty(SearchString) ||
                             FiltroDepartamentoId.HasValue ||
                             FiltroFuncaoId.HasValue;

            // 3. Regra de Negócio: Só busca na API se tiver filtro OU a flag "Todos"
            if (temFiltro || ExibirTodos)
            {
                Colaboradores = await _apiClient.GetColaboradoresAsync(SearchString, FiltroDepartamentoId, FiltroFuncaoId);
                BuscaRealizada = true;
            }
            else
            {
                // Inicia vazio para não pesar o carregamento inicial
                Colaboradores = new List<ColaboradorViewModel>();
                BuscaRealizada = false;
            }
        }
    }
}