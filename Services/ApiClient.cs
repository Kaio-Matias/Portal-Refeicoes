using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Portal_Refeicoes.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace Portal_Refeicoes.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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

        // =================================================================================
        // COLABORADORES (Atualizado: Sem envio de imagem e permissões)
        // =================================================================================

        public async Task<List<ColaboradorViewModel>> GetColaboradoresAsync(string? searchString = null, int? departamentoId = null, int? funcaoId = null)
        {
            AddAuthorizationHeader();

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchString)) queryParams.Add($"searchString={System.Net.WebUtility.UrlEncode(searchString)}");
            if (departamentoId.HasValue) queryParams.Add($"departamentoId={departamentoId.Value}");
            if (funcaoId.HasValue) queryParams.Add($"funcaoId={funcaoId.Value}");

            var url = "api/colaboradores";
            if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

            try
            {
                return await _httpClient.GetFromJsonAsync<List<ColaboradorViewModel>>(url, _jsonOptions) ?? new List<ColaboradorViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar colaboradores.");
                return new List<ColaboradorViewModel>();
            }
        }

        public async Task<ColaboradorViewModel?> GetColaboradorByIdAsync(int id)
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<ColaboradorViewModel>($"api/colaboradores/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar colaborador ID {Id}", id);
                return null;
            }
        }

        public async Task<bool> CreateColaboradorAsync(ColaboradorCreateModel colaborador)
        {
            AddAuthorizationHeader();

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(colaborador.Nome), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");

            var response = await _httpClient.PostAsync("api/colaboradores", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateColaboradorAsync(int id, ColaboradorEditModel colaborador, IFormFile? imagem)
        {
            AddAuthorizationHeader();

            using var content = new MultipartFormDataContent();

            // Adiciona campos de texto
            content.Add(new StringContent(colaborador.Nome ?? ""), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto ?? ""), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");
            content.Add(new StringContent(colaborador.Ativo.ToString()), "Ativo");

            // Adiciona o Arquivo de Imagem, se existir
            if (imagem != null && imagem.Length > 0)
            {
                var fileContent = new StreamContent(imagem.OpenReadStream());
                // Define o tipo de conteúdo (ex: image/jpeg)
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);
                // O nome do campo "imagem" DEVE ser igual ao parâmetro no Controller da API
                content.Add(fileContent, "imagem", imagem.FileName);
            }

            var response = await _httpClient.PutAsync($"api/colaboradores/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // =================================================================================
        // USUÁRIOS (RESTAURADO)
        // =================================================================================

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Usuario>>("api/Usuarios", _jsonOptions) ?? new List<Usuario>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários.");
                return new List<Usuario>();
            }
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
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

        // Nota: Usando 'object' ou os DTOs específicos de Create/Edit que você já tenha no projeto
        public async Task<bool> CreateUsuarioAsync(object usuario)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios", usuario);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUsuarioAsync(int id, object usuario)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Usuarios/{id}", usuario);
            return response.IsSuccessStatusCode;
        }

        // =================================================================================
        // REFEIÇÕES (RESTAURADO)
        // =================================================================================

        public async Task<List<RefeicaoViewModel>> GetRefeicoesAsync(DateTime? data = null, string? tipoRefeicao = null)
        {
            AddAuthorizationHeader();

            var queryParams = new List<string>();

            if (data.HasValue)
            {
                // Formato yyyy-MM-dd para garantir compatibilidade na URL
                queryParams.Add($"data={data.Value:yyyy-MM-dd}");
            }

            if (!string.IsNullOrEmpty(tipoRefeicao))
            {
                queryParams.Add($"tipoRefeicao={System.Net.WebUtility.UrlEncode(tipoRefeicao)}");
            }

            var url = "api/Refeicoes";
            if (queryParams.Any())
            {
                url += "?" + string.Join("&", queryParams);
            }

            try
            {
                return await _httpClient.GetFromJsonAsync<List<RefeicaoViewModel>>(url, _jsonOptions) ?? new List<RefeicaoViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeições.");
                return new List<RefeicaoViewModel>();
            }
        }

        // =================================================================================
        // AUXILIARES (Departamentos e Funções)
        // =================================================================================

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
        // DASHBOARD & JUSTIFICATIVAS
        // =================================================================================

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            AddAuthorizationHeader();
            try { return await _httpClient.GetFromJsonAsync<DashboardStatsViewModel>("api/dashboard/stats", _jsonOptions); }
            catch { return new DashboardStatsViewModel(); }
        }

        public async Task<List<RegistroRecenteViewModel>> GetRegistrosRecentesAsync()
        {
            AddAuthorizationHeader();
            try { return await _httpClient.GetFromJsonAsync<List<RegistroRecenteViewModel>>("api/dashboard/registrosrecentes", _jsonOptions); }
            catch { return new List<RegistroRecenteViewModel>(); }
        }

        public async Task<List<JustificativaPendenciaViewModel>> GetJustificativasPendentesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync("api/Justificativas/pendentes");
                if (!response.IsSuccessStatusCode) return new List<JustificativaPendenciaViewModel>();
                return await response.Content.ReadFromJsonAsync<List<JustificativaPendenciaViewModel>>(_jsonOptions) ?? new List<JustificativaPendenciaViewModel>();
            }
            catch { return new List<JustificativaPendenciaViewModel>(); }
        }

        public async Task<bool> EnviarJustificativaAsync(object dados)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Justificativas", dados);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateDepartamentoAsync(Departamento departamento)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/departamentos", departamento);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDepartamentoAsync(int id, Departamento departamento)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/departamentos/{id}", departamento);
            return response.IsSuccessStatusCode;
        }

        public async Task<Departamento> GetDepartamentoByIdAsync(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<Departamento>($"api/departamentos/{id}", _jsonOptions);
        }

        public async Task<bool> DeleteDepartamentoAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/departamentos/{id}");
            return response.IsSuccessStatusCode;
        }

        // FUNÇÕES
        public async Task<bool> CreateFuncaoAsync(Funcao funcao)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/funcoes", funcao);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateFuncaoAsync(int id, Funcao funcao)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/funcoes/{id}", funcao);
            return response.IsSuccessStatusCode;
        }

        public async Task<Funcao> GetFuncaoByIdAsync(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<Funcao>($"api/funcoes/{id}", _jsonOptions);
        }

        public async Task<bool> DeleteFuncaoAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/funcoes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}