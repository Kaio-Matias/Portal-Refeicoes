using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Funcoes
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public DeleteModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public Funcao Funcao { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Funcao = await _apiClient.GetFuncaoByIdAsync(id);

            if (Funcao == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // O ID vem do input hidden no formulário, vinculado via [BindProperty]
            var sucesso = await _apiClient.DeleteFuncaoAsync(Funcao.Id);

            if (sucesso)
            {
                return RedirectToPage("./Index");
            }

            // Se falhar, recarrega os dados para exibir a página com erro
            Funcao = await _apiClient.GetFuncaoByIdAsync(Funcao.Id);
            ModelState.AddModelError(string.Empty, "Erro ao excluir a função. Ela pode estar em uso por colaboradores.");
            return Page();
        }
    }
}