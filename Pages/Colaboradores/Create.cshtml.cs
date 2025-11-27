using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Colaboradores
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public CreateModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public ColaboradorCreateModel Input { get; set; }

        public SelectList Departamentos { get; set; }
        public SelectList Funcoes { get; set; }

        public async Task OnGetAsync()
        {
            await CarregarListas();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarListas();
                return Page();
            }

            // CORREÇÃO CS1501: Chamada com apenas 1 argumento (o objeto Input)
            var sucesso = await _apiClient.CreateColaboradorAsync(Input);

            if (sucesso)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Erro ao criar colaborador.");
            await CarregarListas();
            return Page();
        }

        private async Task CarregarListas()
        {
            var deps = await _apiClient.GetDepartamentosAsync();
            var funcs = await _apiClient.GetFuncoesAsync();
            Departamentos = new SelectList(deps, "Id", "Nome");
            Funcoes = new SelectList(funcs, "Id", "Nome");
        }
    }
}