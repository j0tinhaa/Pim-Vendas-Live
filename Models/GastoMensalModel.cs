using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStore.Models
{
    public class GastoMensalModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a descrição do gasto.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o valor do gasto.")]
        [Range(0.01, 999999.99, ErrorMessage = "Valor inválido.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Valor (R$)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Informe a data do gasto.")]
        [Display(Name = "Data")]
        public DateTime Data { get; set; } = DateTime.Today;
    }
}
