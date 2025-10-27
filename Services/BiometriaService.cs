using Portal_Refeicoes.Models;
using System.Text.Json;

namespace Portal_Refeicoes.Services
{
    public class BiometriaService
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<BiometriaService> _logger;

        public BiometriaService(ApiClient apiClient, ILogger<BiometriaService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<(bool Sucesso, string Mensagem)> CadastrarBiometriaAsync(int colaboradorId, string templateBase64)
        {
            if (string.IsNullOrEmpty(templateBase64))
            {
                return (false, "O template biométrico não pode estar vazio.");
            }

            var requestModel = new CadastroBiometriaRequest
            {
                ColaboradorId = colaboradorId,
                BiometriaTemplateBase64 = templateBase64
            };

            try
            {
                // Usa o ApiClient para fazer a requisição POST para a API de Refeições
                var (sucesso, responseBody) = await _apiClient.PostAsync("api/biometria/cadastrar", requestModel);

                if (sucesso)
                {
                    _logger.LogInformation("Biometria cadastrada com sucesso para o Colaborador ID: {ColaboradorId}", colaboradorId);

                    // Desserializa a resposta para pegar a mensagem
                    var jsonResponse = JsonDocument.Parse(responseBody);
                    var message = jsonResponse.RootElement.TryGetProperty("message", out var messageElement)
                                  ? messageElement.GetString()
                                  : "Biometria cadastrada com sucesso.";

                    return (true, message ?? "Operação realizada com sucesso.");
                }
                else
                {
                    _logger.LogWarning("Falha ao cadastrar biometria para o Colaborador ID: {ColaboradorId}. Resposta: {Resposta}", colaboradorId, responseBody);
                    return (false, $"Erro da API: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao tentar cadastrar biometria para o Colaborador ID: {ColaboradorId}", colaboradorId);
                return (false, $"Erro de exceção: {ex.Message}");
            }
        }
    }
}