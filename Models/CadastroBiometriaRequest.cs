namespace Portal_Refeicoes.Models
{
    // DTO para enviar a biometria para a API
    public class CadastroBiometriaRequest
    {
        public int ColaboradorId { get; set; }
        public string BiometriaTemplateBase64 { get; set; }
    }
}