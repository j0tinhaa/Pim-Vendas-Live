using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PimVendas.Models
{
    public class VendaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome da cliente!")]
        [StringLength(100, ErrorMessage = "Nome não pode ultrapassar 100 caracteres.")]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o código do produto!")]
        [StringLength(30, ErrorMessage = "Código não pode ultrapassar 30 caracteres.")]
        [Display(Name = "Código do Produto")]
        public string CodigoProduto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome do produto!")]
        [StringLength(150, ErrorMessage = "Nome do produto não pode ultrapassar 150 caracteres.")]
        [Display(Name = "Produto")]
        public string Produto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o valor da venda!")]
        [Range(0.01, 99999.99, ErrorMessage = "O valor deve estar entre R$ 0,01 e R$ 99.999,99.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Valor (R$)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Informe o status da venda!")]
        [Display(Name = "Status")]
        public StatusVenda Status { get; set; } = StatusVenda.Pendente;

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "Observações não podem ultrapassar 500 caracteres.")]
        public string? Observacoes { get; set; }

        [Display(Name = "Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Display(Name = "Última Atualização")]
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;
    }

    public enum StatusVenda
    {
        [Display(Name = "Pendente")]
        Pendente = 0,

        [Display(Name = "Pago")]
        Pago = 1,

        [Display(Name = "Cancelado")]
        Cancelado = 2,

        [Display(Name = "Entregue")]
        Entregue = 3
    }
}
