using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Portal_Refeicoes.Models
{
    // LISTAGEM (Index)
    public class ColaboradorViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CartaoPonto { get; set; }
        public bool Ativo { get; set; }
        public byte[] Foto { get; set; } // Para exibir na lista

        public string Funcao { get; set; }
        public string Departamento { get; set; }
        public int FuncaoId { get; set; }
        public int DepartamentoId { get; set; }
    }

    // CADASTRO (Create)
    public class ColaboradorCreateModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O Cartão Ponto é obrigatório.")]
        public string CartaoPonto { get; set; }

        [Required(ErrorMessage = "Selecione uma Função.")]
        public int FuncaoId { get; set; }

        [Required(ErrorMessage = "Selecione um Departamento.")]
        public int DepartamentoId { get; set; }

        // [RESTAURADO] Campo para upload da foto
        [Display(Name = "Foto do Colaborador")]
        public IFormFile Foto { get; set; }
    }

    // EDIÇÃO (Edit)
    public class ColaboradorEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O Cartão Ponto é obrigatório.")]
        public string CartaoPonto { get; set; }

        [Required]
        public int FuncaoId { get; set; }

        [Required]
        public int DepartamentoId { get; set; }

        public bool Ativo { get; set; }

        // [RESTAURADO] Opcional na edição
        [Display(Name = "Alterar Foto")]
        public IFormFile Foto { get; set; }
    }
}