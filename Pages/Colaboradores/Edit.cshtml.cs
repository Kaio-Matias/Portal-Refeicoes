using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Colaboradores
{
    public class EditModel : PageModel
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApiClient apiClient, ILogger<EditModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [BindProperty]
        public ColaboradorEditModel Colaborador { get; set; } = new ColaboradorEditModel();

        // Propriedade que recebe o arquivo do formulário
        [BindProperty]
        public IFormFile? ImagemFile { get; set; }

        public List<SelectListItem> Departamentos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Funcoes { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var colaboradorViewModel = await _apiClient.GetColaboradorByIdAsync(id.Value);
            if (colaboradorViewModel == null) return NotFound();

            Colaborador = new ColaboradorEditModel
            {
                Id = colaboradorViewModel.Id,
                Nome = colaboradorViewModel.Nome,
                CartaoPonto = colaboradorViewModel.CartaoPonto,
                DepartamentoId = colaboradorViewModel.DepartamentoId,
                FuncaoId = colaboradorViewModel.FuncaoId,
                Ativo = colaboradorViewModel.Ativo,
                // FotoAtual serve apenas para exibição se necessário, não é enviada de volta
            };

            await LoadSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validação de campos que não vêm do form
            ModelState.Remove("Colaborador.Foto");

            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            _logger.LogInformation("Enviando atualização para ID: {Id}. Imagem presente: {HasImage}",
                Colaborador.Id, ImagemFile != null);

            // Passa o ImagemFile explicitamente
            var success = await _apiClient.UpdateColaboradorAsync(Colaborador.Id, Colaborador, ImagemFile);

            if (success)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar colaborador na API.");
                await LoadSelectLists();
                return Page();
            }
        }

        private async Task LoadSelectLists()
        {
            var departamentos = await _apiClient.GetDepartamentosAsync();
            var funcoes = await _apiClient.GetFuncoesAsync();

            Departamentos = departamentos?.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome }).ToList() ?? new List<SelectListItem>();
            Funcoes = funcoes?.Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nome }).ToList() ?? new List<SelectListItem>();
        }
    }
}