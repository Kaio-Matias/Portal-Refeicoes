using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Portal_Refeicoes.Models;
using Portal_Refeicoes.Pages.Usuarios;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Adicionado
using System.Text.Json; // Adicionado
using System.Text; // Adicionado
using System; // Adicionado

namespace Portal_Refeicoes.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _apiBaseUrl;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // --- INÍCIO DA CORREÇÃO ---
            // Corrigido para ler a chave "ApiSettings:ApiBaseUrl" do appsettings.json
            _apiBaseUrl = configuration["ApiSettings:ApiBaseUrl"] ?? "https://localhost:7071"; // Fallback corrigido
            // --- FIM DA CORREÇÃO ---

            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
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

        // --- MÉTODOS DE USUÁRIO (Originais Preservados) ---

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Usuario>>("api/Usuarios");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários da API.");
                return new List<Usuario>();
            }
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"api/Usuarios/{id}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar o usuário com ID {UserId} da API.", id);
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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro ao criar usuário: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return false;
            }
            return true;
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

        // --- MÉTODOS DE REFEIÇÃO (Originais Preservados) ---

        public async Task<List<RefeicaoViewModel>> GetRefeicoesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RefeicaoViewModel>>("api/Refeicoes");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar refeições da API.");
                return new List<RefeicaoViewModel>();
            }
        }

        // --- MÉTODOS DE COLABORADOR (Atualizados) ---

        public async Task<List<ColaboradorViewModel>> GetColaboradoresAsync(string searchString = null)
        {
            AddAuthorizationHeader();
            var url = "api/colaboradores";
            if (!string.IsNullOrEmpty(searchString))
            {
                url += $"?searchString={System.Net.WebUtility.UrlEncode(searchString)}";
            }
            return await _httpClient.GetFromJsonAsync<List<ColaboradorViewModel>>(url) ?? new List<ColaboradorViewModel>();
        }

        public async Task<ColaboradorViewModel> GetColaboradorByIdAsync(int id)
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ColaboradorViewModel>($"api/colaboradores/{id}");
        }

        public async Task<bool> CreateColaboradorAsync(ColaboradorCreateModel colaborador, IFormFile imagem)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(colaborador.Nome), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");

            // --- ADIÇÃO DOS CAMPOS DE PERMISSÃO PARA CRIAÇÃO ---
            content.Add(new StringContent(colaborador.AcessoCafeDaManha.ToString().ToLower()), "AcessoCafeDaManha");
            content.Add(new StringContent(colaborador.AcessoAlmoco.ToString().ToLower()), "AcessoAlmoco");
            content.Add(new StringContent(colaborador.AcessoJanta.ToString().ToLower()), "AcessoJanta");
            content.Add(new StringContent(colaborador.AcessoCeia.ToString().ToLower()), "AcessoCeia");

            if (imagem != null)
                content.Add(new StreamContent(imagem.OpenReadStream()), "imagem", imagem.FileName);

            var response = await _httpClient.PostAsync("api/colaboradores", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Falha ao criar colaborador. Status: {StatusCode}", response.StatusCode);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateColaboradorAsync(int id, ColaboradorEditModel colaborador, IFormFile? imagem)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(colaborador.Nome), "Nome");
            content.Add(new StringContent(colaborador.CartaoPonto), "CartaoPonto");
            content.Add(new StringContent(colaborador.FuncaoId.ToString()), "FuncaoId");
            content.Add(new StringContent(colaborador.DepartamentoId.ToString()), "DepartamentoId");
            content.Add(new StringContent(colaborador.Ativo.ToString().ToLower()), "Ativo");

            // --- ADIÇÃO DOS CAMPOS DE PERMISSÃO PARA ATUALIZAÇÃO ---
            content.Add(new StringContent(colaborador.AcessoCafeDaManha.ToString().ToLower()), "AcessoCafeDaManha");
            content.Add(new StringContent(colaborador.AcessoAlmoco.ToString().ToLower()), "AcessoAlmoco");
            content.Add(new StringContent(colaborador.AcessoJanta.ToString().ToLower()), "AcessoJanta");
            content.Add(new StringContent(colaborador.AcessoCeia.ToString().ToLower()), "AcessoCeia");

            if (imagem != null)
                content.Add(new StreamContent(imagem.OpenReadStream()), "imagem", imagem.FileName);

            var response = await _httpClient.PutAsync($"api/colaboradores/{id}", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Falha ao atualizar colaborador. Status: {StatusCode}", response.StatusCode);
            }
            return response.IsSuccessStatusCode;
        }

        // --- MÉTODOS DE DEPARTAMENTO E FUNÇÃO (Originais Preservados) ---

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Departamento>>("api/departamentos") ?? new List<Departamento>();
        }

        public async Task<List<Funcao>> GetFuncoesAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Funcao>>("api/funcoes") ?? new List<Funcao>();
        }

        // --- MÉTODOS DE DASHBOARD (Originais Preservados) ---

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardStatsViewModel>("api/dashboard/stats");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard.");
                return new DashboardStatsViewModel();
            }
        }

        public async Task<List<RegistroRecenteViewModel>> GetRegistrosRecentesAsync()
        {
            AddAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RegistroRecenteViewModel>>("api/dashboard/registrosrecentes");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar registros recentes do dashboard.");
                return new List<RegistroRecenteViewModel>();
            }
        }

        // --- MÉTODO DE BIOMETRIA (CORRIGIDO E ROBUSTO) ---
        public async Task<(bool Success, string Message)> CadastrarBiometriaAsync(CadastroBiometriaRequest request)
        {
            AddAuthorizationHeader();

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/biometria/cadastrar", httpContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    string message = result.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Sucesso.";
                    return (true, message);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Não foi possível deserializar a resposta de sucesso da API de biometria.");
                    return (true, "Biometria cadastrada com sucesso.");
                }
            }
            else
            {
                try
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    string message = result.TryGetProperty("message", out var msgElement) ? msgElement.GetString() :
                                     result.TryGetProperty("title", out var titleElement) ? titleElement.GetString() :
                                     response.ReasonPhrase;
                    _logger.LogError("Erro da API ao cadastrar biometria: {Message}", message);
                    return (false, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro desconhecido ao cadastrar biometria. Status: {StatusCode}", response.StatusCode);
                    return (false, $"Erro: {response.ReasonPhrase} ({(int)response.StatusCode})");
                }
            }
        }
    }
}