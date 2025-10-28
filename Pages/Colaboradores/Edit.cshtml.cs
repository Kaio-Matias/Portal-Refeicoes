using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Portal_Refeicoes.Models; // <-- Garante que está usando o namespace Models
using Portal_Refeicoes.Services;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations; // Não é mais necessário aqui se o modelo está em Models
using System.Linq;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Colaboradores
{
    // --- A DEFINIÇÃO DA CLASSE ColaboradorEditModel FOI REMOVIDA DAQUI ---

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
        public ColaboradorEditModel Colaborador { get; set; } = new ColaboradorEditModel(); // Agora refere-se a Models.ColaboradorEditModel

        [BindProperty]
        public IFormFile? ImagemFile { get; set; } // Upload fica aqui no PageModel

        public List<SelectListItem> Departamentos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Funcoes { get; set; } = new List<SelectListItem>();


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colaboradorViewModel = await _apiClient.GetColaboradorByIdAsync(id.Value);
            if (colaboradorViewModel == null)
            {
                return NotFound();
            }

            // Mapeia do ViewModel para o EditModel (Models.ColaboradorEditModel)
            Colaborador = new ColaboradorEditModel
            {
                Id = colaboradorViewModel.Id,
                Nome = colaboradorViewModel.Nome,
                CartaoPonto = colaboradorViewModel.CartaoPonto,
                DepartamentoId = colaboradorViewModel.DepartamentoId,
                FuncaoId = colaboradorViewModel.FuncaoId,
                Ativo = colaboradorViewModel.Ativo,
                FotoAtual = colaboradorViewModel.Foto, // Mapeia a propriedade Foto (byte[])
                AcessoCafeDaManha = colaboradorViewModel.AcessoCafeDaManha,
                AcessoAlmoco = colaboradorViewModel.AcessoAlmoco,
                AcessoJanta = colaboradorViewModel.AcessoJanta,
                AcessoCeia = colaboradorViewModel.AcessoCeia
            };


            await LoadSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
             ModelState.Remove($"{nameof(Colaborador)}.{nameof(Colaborador.FotoAtual)}");

            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                if (Colaborador.Id > 0 && (Colaborador.FotoAtual == null || Colaborador.FotoAtual.Length == 0)) {
                    var currentViewModel = await _apiClient.GetColaboradorByIdAsync(Colaborador.Id);
                    Colaborador.FotoAtual = currentViewModel?.Foto;
                }
                return Page();
            }

            _logger.LogInformation("Iniciando atualização do colaborador ID: {ColaboradorId}", Colaborador.Id);

            // --- CORREÇÃO: Agora a variável 'Colaborador' é do tipo correto (Models.ColaboradorEditModel) ---
            var success = await _apiClient.UpdateColaboradorAsync(Colaborador.Id, Colaborador, ImagemFile);

            if (success)
            {
                _logger.LogInformation("Colaborador ID: {ColaboradorId} atualizado com sucesso.", Colaborador.Id);
                TempData["SuccessMessage"] = "Colaborador atualizado com sucesso!";
                return RedirectToPage("./Index");
            }
            else
            {
                 _logger.LogWarning("Falha ao atualizar colaborador ID: {ColaboradorId} via API.", Colaborador.Id);
                ModelState.AddModelError(string.Empty, "Erro ao atualizar colaborador. Tente novamente.");
                await LoadSelectLists();
                var currentViewModel = await _apiClient.GetColaboradorByIdAsync(Colaborador.Id);
                Colaborador.FotoAtual = currentViewModel?.Foto;
                return Page();
            }
        }


        // HANDLER PARA BIOMETRIA (Não precisa mudar, já usava o ID correto)
        public async Task<IActionResult> OnPostCadastrarBiometriaAsync([FromBody] CadastroBiometriaRequest request)
        {
             if (request == null || request.ColaboradorId == 0 || string.IsNullOrEmpty(request.BiometriaTemplateBase64))
            {
                _logger.LogWarning("Recebido pedido inválido para cadastrar biometria: {@Request}", request);
                return new JsonResult(new { success = false, message = "Dados inválidos recebidos." }) { StatusCode = 400 };
            }

             // Garante que temos o ID do colaborador corretamente, mesmo que o BindProperty principal ainda não tenha ocorrido
             int colaboradorIdParaBiometria = Colaborador?.Id ?? 0;
             if (colaboradorIdParaBiometria == 0 && RouteData.Values.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out int routeId))
             {
                 colaboradorIdParaBiometria = routeId;
             }

             if(colaboradorIdParaBiometria == 0) {
                 _logger.LogError("Não foi possível determinar o ID do colaborador na página de edição para o cadastro de biometria.");
                 return new JsonResult(new { success = false, message = "ID do colaborador não encontrado." }) { StatusCode = 400 };
             }


            if (request.ColaboradorId != colaboradorIdParaBiometria)
            {
                 _logger.LogWarning("Inconsistência de ID ao cadastrar biometria. ID da Página: {PageId}, ID do Request: {RequestId}", colaboradorIdParaBiometria, request.ColaboradorId);
                 return new JsonResult(new { success = false, message = "Inconsistência no ID do colaborador." }) { StatusCode = 400 };
            }

            _logger.LogInformation("Recebido POST para cadastrar biometria para Colaborador ID {ColaboradorId}", request.ColaboradorId);

            try
            {
                var (success, message) = await _apiClient.CadastrarBiometriaAsync(request);

                if (success)
                {
                    _logger.LogInformation("Biometria cadastrada com sucesso via API para Colaborador ID {ColaboradorId}", request.ColaboradorId);
                    return new JsonResult(new { success = true, message = message ?? "Biometria cadastrada com sucesso!" });
                }
                else
                {
                    _logger.LogWarning("Falha ao cadastrar biometria via API para Colaborador ID {ColaboradorId}. Mensagem: {ApiMessage}", request.ColaboradorId, message);
                    return new JsonResult(new { success = false, message = message ?? "Falha ao cadastrar biometria na API." }) { StatusCode = 400 };
                }
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Exceção no handler OnPostCadastrarBiometriaAsync para Colaborador ID {ColaboradorId}", request.ColaboradorId);
                 return new JsonResult(new { success = false, message = "Erro interno no servidor ao processar a biometria." }) { StatusCode = 500 };
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