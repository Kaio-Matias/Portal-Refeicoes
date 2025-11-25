using System;
using System.ComponentModel.DataAnnotations;

namespace Portal_Refeicoes.Models
{
    // Modelo para exibir na tabela de pendências
    public class JustificativaPendenciaViewModel
    {
        public int RegistroRefeicaoId { get; set; }
        public string NomeColaborador { get; set; }
        public string Departamento { get; set; }
        public string TipoRefeicao { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Valor { get; set; }
    }

    // Modelo para o formulário de envio
    public class CriarJustificativaViewModel
    {
        [Required]
        public int RegistroRefeicaoId { get; set; }

        [Required(ErrorMessage = "Por favor, informe o motivo.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "A justificativa deve ter entre 5 e 500 caracteres.")]
        [Display(Name = "Motivo da Justificativa")]
        public string Motivo { get; set; }
    }
}