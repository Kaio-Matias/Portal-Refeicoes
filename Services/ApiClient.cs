using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Pages.Usuarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
// Adicionado JsonSerializerOptions para garantir compatibilidade de nomes (camelCase vs PascalCase)
using System.Text.Json;

namespace Portal_Refeicoes.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiClient> _logger;

        // Opções de JSON para garantir que a deserialização funcione mesmo se a API retornar camelCase
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ... (Métodos de Usuário, Refeição e Colaborador mantidos iguais) ...
        // (Estou omitindo para focar na correção, mas eles devem permanecer no arquivo final)

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Usuario>>("api/Usuarios", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários.");
                return new List<Usuario>();
            }
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"api/Usuarios/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário ID {Id}", id);
                return null;
            }
        }

        public async Task<bool> CreateUsuarioAsync(CreateModel.InputModel usuario)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", new
            {
                Username = usuario.Username,
                Password = usuario.Password,
                Role = usuario.Role
            });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUsuarioAsync(int id, EditModel.InputModel usuario)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", new
            {
                Username = usuario.Username,
                Role = usuario.Role,
                Password = usuario.Password
            });
            return response.IsSuccessStatusCode;
        }

        public async Task<List<RefeicaoViewModel>> GetRefeicoesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RefeicaoViewModel>>("api/Refeicoes", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeições.");
                return new List<RefeicaoViewModel>();
            }
        }

        public async Task<List<ColaboradorViewModel>> GetColaboradoresAsync(string searchString = null)
        {
            AddAuthorizationHeader();
            var url = "api/colaboradores";
            if (!string.IsNullOrEmpty(searchString))
            {
                url += $"?searchString={System.Net.WebUtility.UrlEncode(searchString)}";
            }
            return await _httpClient.GetFromJsonAsync<List<ColaboradorViewModel>>(url, _jsonOptions) ?? new List<ColaboradorViewModel>();
        }

        public async Task<ColaboradorViewModel> GetColaboradorByIdAsync(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ColaboradorViewModel>($"api/colaboradores/{id}", _jsonOptions);
        }

        public async Task<bool> CreateColaboradorAsync(ColaboradorCreateModel colaborador, IFormFile imagem)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            // ... (lógica de multipart mantida, apenas verifique se os nomes dos campos batem com a API)
            content.Add(new StringContent(colaborador.Nome), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");
            content.Add(new StringContent(colaborador.AcessoCafeDaManha.ToString().ToLower()), "AcessoCafeDaManha");
            content.Add(new StringContent(colaborador.AcessoAlmoco.ToString().ToLower()), "AcessoAlmoco");
            content.Add(new StringContent(colaborador.AcessoJanta.ToString().ToLower()), "AcessoJanta");
            content.Add(new StringContent(colaborador.AcessoCeia.ToString().ToLower()), "AcessoCeia");

            if (imagem != null)
                content.Add(new StreamContent(imagem.OpenReadStream()), "imagem", imagem.FileName);

            var response = await _httpClient.PostAsync("api/colaboradores", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateColaboradorAsync(int id, ColaboradorEditModel colaborador, IFormFile? imagem)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            // ... (lógica mantida)
            content.Add(new StringContent(colaborador.Nome), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");
            content.Add(new StringContent(colaborador.Ativo.ToString().ToLower()), "Ativo");
            content.Add(new StringContent(colaborador.AcessoCafeDaManha.ToString().ToLower()), "AcessoCafeDaManha");
            content.Add(new StringContent(colaborador.AcessoAlmoco.ToString().ToLower()), "AcessoAlmoco");
            content.Add(new StringContent(colaborador.AcessoJanta.ToString().ToLower()), "AcessoJanta");
            content.Add(new StringContent(colaborador.AcessoCeia.ToString().ToLower()), "AcessoCeia");

            if (imagem != null)
                content.Add(new StreamContent(imagem.OpenReadStream()), "imagem", imagem.FileName);

            var response = await _httpClient.PutAsync($"api/colaboradores/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Departamento>>("api/departamentos", _jsonOptions) ?? new List<Departamento>();
        }

        public async Task<List<Funcao>> GetFuncoesAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Funcao>>("api/funcoes", _jsonOptions) ?? new List<Funcao>();
        }


        // =================================================================================
        // MÉTODOS DE DASHBOARD
        // =================================================================================

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            AddAuthorizationHeader();
            try
            {
                // Passamos _jsonOptions para evitar problemas de case sensitivity (alertasPendentes vs AlertasPendentes)
                return await _httpClient.GetFromJsonAsync<DashboardStatsViewModel>("api/dashboard/stats", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do dashboard.");
                return new DashboardStatsViewModel();
            }
        }

        public async Task<List<RegistroRecenteViewModel>> GetRegistrosRecentesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RegistroRecenteViewModel>>("api/dashboard/registrosrecentes", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter registros recentes.");
                return new List<RegistroRecenteViewModel>();
            }
        }

        // =================================================================================
        // MÉTODOS DE JUSTIFICATIVAS (CORRIGIDO)
        // =================================================================================

        public async Task<List<JustificativaPendenciaViewModel>> GetJustificativasPendentesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                // CORREÇÃO 1: Log para debug
                _logger.LogInformation("Chamando API: api/Justificativas/pendentes");

                var response = await _httpClient.GetAsync("api/Justificativas/pendentes");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API retornou status não-sucesso para pendências: {StatusCode}", response.StatusCode);
                    return new List<JustificativaPendenciaViewModel>();
                }

                // CORREÇÃO 2: Usar ReadFromJsonAsync com Options para garantir deserialização
                var result = await response.Content.ReadFromJsonAsync<List<JustificativaPendenciaViewModel>>(_jsonOptions);

                _logger.LogInformation("Recebidos {Count} itens de pendência.", result?.Count ?? 0);

                return result ?? new List<JustificativaPendenciaViewModel>();
            }
            catch (Exception ex)
            {
                // CORREÇÃO 3: Logar o erro real para o console
                _logger.LogError(ex, "EXCEÇÃO ao obter justificativas pendentes.");
                return new List<JustificativaPendenciaViewModel>();
            }
        }

        public async Task<bool> EnviarJustificativaAsync(object dadosJustificativa)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Justificativas", dadosJustificativa);
                if (!response.IsSuccessStatusCode)
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro ao enviar justificativa. Status: {Status}. Msg: {Msg}", response.StatusCode, erro);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao enviar justificativa.");
                return false;
            }
        }
    }
}