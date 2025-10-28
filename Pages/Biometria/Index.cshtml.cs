using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Biometria
{
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public List<SelectListItem> Colaboradores { get; set; } = new List<SelectListItem>();

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // CORREÇÃO: O método chama-se GetColaboradoresAsync
            var colaboradores = await _apiClient.GetColaboradoresAsync();

            if (colaboradores != null)
            {
                Colaboradores = colaboradores
                    // Assumindo que seu ColaboradorViewModel tem a propriedade Ativo
                    // Se não tiver, remova a linha abaixo.
                    .Where(c => c.Ativo)
                    .OrderBy(c => c.Nome)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Nome} (Mat: {c.CartaoPonto ?? "N/A"})"
                    }).ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCadastrarBiometriaAsync([FromBody] CadastroBiometriaRequest request)
        {
            if (request == null || request.ColaboradorId == 0 || string.IsNullOrEmpty(request.BiometriaTemplateBase64))
            {
                return new JsonResult(new { success = false, message = "Dados inválidos." });
            }

            // Esta chamada está correta e usa o ApiClient, não o BiometriaService
            var (success, message) = await _apiClient.CadastrarBiometriaAsync(request);

            if (success)
            {
                return new JsonResult(new { success = true, message = message });
            }
            else
            {
                return new JsonResult(new { success = false, message = message });
            }
        }
    }
}