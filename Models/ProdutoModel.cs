using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStore.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o código do produto!")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome do produto!")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o preço!")]
        [Range(0.01, 99999.99, ErrorMessage = "Preço entre R$ 0,01 e R$ 99.999,99.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Preço (R$)")]
        public decimal Preco { get; set; }

        [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        [Display(Name = "Tipo / Categoria")]
        public string? Tipo { get; set; }

        [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
        [Display(Name = "Cor")]
        public string? Cor { get; set; }

        [StringLength(10, ErrorMessage = "Máximo 10 caracteres.")]
        [Display(Name = "Tamanho")]
        public string? Tamanho { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Cadastrado em")]
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        // Navegação
        public ICollection<VendaModel> Vendas { get; set; } = new List<VendaModel>();
    }
}
