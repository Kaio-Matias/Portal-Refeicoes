using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portal_Refeicoes.Pages.Justificativas
{
    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public IndexModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Lista que vai popular a tabela
        public List<JustificativaPendenciaViewModel> Pendencias { get; set; } = new List<JustificativaPendenciaViewModel>();

        // Propriedade para receber os dados do Modal (formulário)
        [BindProperty]
        public CriarJustificativaViewModel Input { get; set; }

        [TempData]
        public string MensagemSucesso { get; set; }

        [TempData]
        public string MensagemErro { get; set; }

        public async Task OnGetAsync()
        {
            await CarregarDados();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                MensagemErro = "Por favor, preencha o motivo corretamente.";
                await CarregarDados();
                return Page();
            }

            // Monta o objeto anônimo para enviar ao endpoint da API
            // Ajuste "Responsavel" para pegar do User.Identity.Name se tiver autenticação
            var payload = new
            {
                RegistroRefeicaoId = Input.RegistroRefeicaoId,
                Motivo = Input.Motivo,
                Responsavel = User.Identity?.Name ?? "Gestor"
            };

            var sucesso = await _apiClient.EnviarJustificativaAsync(payload);

            if (sucesso)
            {
                MensagemSucesso = "Justificativa registrada com sucesso!";
                return RedirectToPage(); // Padrão PRG para limpar o form e recarregar a lista
            }
            else
            {
                MensagemErro = "Ocorreu um erro ao salvar a justificativa.";
                await CarregarDados();
                return Page();
            }
        }

        private async Task CarregarDados()
        {
            Pendencias = await _apiClient.GetJustificativasPendentesAsync();
        }
    }
}