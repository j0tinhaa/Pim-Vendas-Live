using System.ComponentModel.DataAnnotations;

namespace LiveStore.Models
{
    /// <summary>
    /// Cliente identificado pelo @ do Instagram.
    /// O InstagramUser é a chave primária (string).
    /// </summary>
    public class ClienteModel
    {
        [Key]
        [Required(ErrorMessage = "Informe o @ do Instagram!")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        [Display(Name = "@ Instagram")]
        public string InstagramUser { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Display(Name = "Cadastrado em")]
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        // Navegação
        public ICollection<VendaModel> Vendas { get; set; } = new List<VendaModel>();
    }
}
