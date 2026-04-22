using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStore.Models
{
    public class LiveModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome da live!")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        [Display(Name = "Nome da Live")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Status")]
        public StatusLive Status { get; set; } = StatusLive.Ativa;

        [Display(Name = "Iniciada em")]
        public DateTime IniciadaEm { get; set; } = DateTime.Now;

        [Display(Name = "Encerrada em")]
        public DateTime? EncerradaEm { get; set; }

        // Navegação
        public ICollection<VendaModel> Vendas { get; set; } = new List<VendaModel>();

        // Propriedades calculadas (não mapeadas)
        [NotMapped]
        public decimal FaturamentoTotal =>
            Vendas.Where(v => v.Status != StatusVenda.Cancelado).Sum(v => v.Valor);

        [NotMapped]
        public int TotalVendas => Vendas.Count;
    }

    public enum StatusLive
    {
        [Display(Name = "Ativa")]
        Ativa = 0,

        [Display(Name = "Encerrada")]
        Encerrada = 1
    }
}
