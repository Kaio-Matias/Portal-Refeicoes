using System;
using System.Text.Json.Serialization;

namespace Portal_Refeicoes.Models
{
    public class RegistroRecenteViewModel
    {
        [JsonPropertyName("colaboradorNome")]
        public string ColaboradorNome { get; set; }

        [JsonPropertyName("departamentoNome")]
        public string DepartamentoNome { get; set; }

        [JsonPropertyName("dataHoraRegistro")]
        public DateTime DataHoraRegistro { get; set; }
    }
}