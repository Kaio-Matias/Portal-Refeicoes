using System;
using System.Text.Json.Serialization;

namespace Portal_Refeicoes.Models
{
    // Modelo para os dados simplificados do colaborador
    public class ColaboradorRefeicaoViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("funcaoNome")]
        public string FuncaoNome { get; set; }

        [JsonPropertyName("departamentoNome")]
        public string DepartamentoNome { get; set; }
    }

    // Modelo principal que a página irá usar
    public class RefeicaoViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("dataHoraRegistro")]
        public DateTime DataHoraRegistro { get; set; }

        [JsonPropertyName("colaborador")]
        public ColaboradorRefeicaoViewModel Colaborador { get; set; }
        public byte[] FotoRegistro { get; set; }
        public string? TipoRefeicao { get; set; }  
    }
}