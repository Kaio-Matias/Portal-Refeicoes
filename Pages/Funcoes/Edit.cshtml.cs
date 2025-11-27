using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Funcoes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public EditModel(ApiClient apiClient)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var sucesso = await _apiClient.UpdateFuncaoAsync(Funcao.Id, Funcao);

            if (sucesso)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Erro ao atualizar a função.");
            return Page();
        }
    }
}