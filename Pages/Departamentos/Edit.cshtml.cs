using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Portal_Refeicoes.Pages.Departamentos
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApiClient _apiClient;
        public EditModel(ApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty] public Departamento Departamento { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Departamento = await _apiClient.GetDepartamentoByIdAsync(id);
            if (Departamento == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (await _apiClient.UpdateDepartamentoAsync(Departamento.Id, Departamento)) return RedirectToPage("./Index");
            return Page();
        }
    }
}