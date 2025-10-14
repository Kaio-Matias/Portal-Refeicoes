using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Portal_Refeicoes.Models
{
    // Modelo para EXIBIR a lista de colaboradores
    public class ColaboradorViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Cartão Ponto")]
        public string CartaoPonto { get; set; }
        public string Funcao { get; set; }
        public string Departamento { get; set; }
        public bool Ativo { get; set; }
        public byte[]? Foto { get; set; }
        public int FuncaoId { get; set; }
        public int DepartamentoId { get; set; }

        // --- PROPRIEDADES ADICIONADAS PARA EXIBIÇÃO ---
        [Display(Name = "Café")]
        public bool AcessoCafeDaManha { get; set; }
        [Display(Name = "Almoço")]
        public bool AcessoAlmoco { get; set; }
        [Display(Name = "Janta")]
        public bool AcessoJanta { get; set; }
        [Display(Name = "Ceia")]
        public bool AcessoCeia { get; set; }
    }

    // Modelo para o formulário de CRIAÇÃO
    public class ColaboradorCreateModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O cartão de ponto é obrigatório.")]
        [Display(Name = "Cartão Ponto")]
        public string CartaoPonto { get; set; }

        [Required(ErrorMessage = "A função é obrigatória.")]
        [Display(Name = "Função")]
        public int FuncaoId { get; set; }

        [Required(ErrorMessage = "O departamento é obrigatório.")]
        [Display(Name = "Departamento")]
        public int DepartamentoId { get; set; }

        // --- PROPRIEDADES ADICIONADAS PARA CRIAÇÃO ---
        [Display(Name = "Acesso ao Café da Manhã")]
        public bool AcessoCafeDaManha { get; set; }
        [Display(Name = "Acesso ao Almoço")]
        public bool AcessoAlmoco { get; set; }
        [Display(Name = "Acesso à Janta")]
        public bool AcessoJanta { get; set; }
        [Display(Name = "Acesso à Ceia")]
        public bool AcessoCeia { get; set; }
    }

    // Modelo para o formulário de EDIÇÃO
    public class ColaboradorEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O cartão de ponto é obrigatório.")]
        [Display(Name = "Cartão Ponto")]
        public string CartaoPonto { get; set; }

        [Required(ErrorMessage = "A função é obrigatória.")]
        [Display(Name = "Função")]
        public int FuncaoId { get; set; }

        [Required(ErrorMessage = "O departamento é obrigatório.")]
        [Display(Name = "Departamento")]
        public int DepartamentoId { get; set; }

        public bool Ativo { get; set; }

        public byte[]? FotoAtual { get; set; }

        // --- PROPRIEDADES ADICIONADAS PARA EDIÇÃO ---
        [Display(Name = "Acesso ao Café da Manhã")]
        public bool AcessoCafeDaManha { get; set; }
        [Display(Name = "Acesso ao Almoço")]
        public bool AcessoAlmoco { get; set; }
        [Display(Name = "Acesso à Janta")]
        public bool AcessoJanta { get; set; }
        [Display(Name = "Acesso à Ceia")]
        public bool AcessoCeia { get; set; }
    }
}