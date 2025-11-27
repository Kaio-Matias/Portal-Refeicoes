using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;

namespace Portal_Refeicoes.Pages.Departamentos
{
    public class CreateModel : PageModel
    {
        private readonly ApiClient _apiClient;
        public CreateModel(ApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty] public Departamento Departamento { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (await _apiClient.CreateDepartamentoAsync(Departamento)) return RedirectToPage("./Index");
            ModelState.AddModelError("", "Erro ao criar.");
            return Page();
        }
    }
}