using System.Text.Json.Serialization;

namespace Portal_Refeicoes.Models
{
    public class DashboardStatsViewModel
    {
        [JsonPropertyName("totalColaboradoresAtivos")]
        public int TotalColaboradoresAtivos { get; set; }

        [JsonPropertyName("refeicoesHoje")]
        public int RefeicoesHoje { get; set; }
        public int AlertasPendentes { get; set; }
    }
}