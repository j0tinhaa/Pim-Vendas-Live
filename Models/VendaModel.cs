using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStore.Models
{
    public class VendaModel
    {
        public int Id { get; set; }

        // FK — Live
        [Required]
        [Display(Name = "Live")]
        public int LiveId { get; set; }
        public LiveModel? Live { get; set; }

        // FK — Cliente (@ Instagram)
        [Required(ErrorMessage = "Informe o @ da cliente!")]
        [StringLength(50)]
        [Display(Name = "@ Instagram")]
        public string ClienteInstagram { get; set; } = string.Empty;
        public ClienteModel? Cliente { get; set; }

        // FK — Produto (opcional: pode digitar código manualmente)
        [Display(Name = "Produto")]
        public int? ProdutoId { get; set; }
        public ProdutoModel? Produto { get; set; }

        // Campos desnormalizados para histórico
        // (preserva dados mesmo se produto for editado depois)
        [Required(ErrorMessage = "Informe o código do produto!")]
        [StringLength(30)]
        [Display(Name = "Código do Produto")]
        public string CodigoProduto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome do produto!")]
        [StringLength(150)]
        [Display(Name = "Produto")]
        public string NomeProduto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o valor!")]
        [Range(0.01, 99999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 99.999,99.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Valor (R$)")]
        public decimal Valor { get; set; }

        [Display(Name = "Status")]
        public StatusVenda Status { get; set; } = StatusVenda.Reservado;

        [StringLength(500)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Display(Name = "Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Display(Name = "Última Atualização")]
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;
    }

    public enum StatusVenda
    {
        [Display(Name = "Reservado")]
        Reservado = 0,

        [Display(Name = "Pago")]
        Pago = 1,

        [Display(Name = "Entregue")]
        Entregue = 2,

        [Display(Name = "Cancelado")]
        Cancelado = 3
    }
}
