using System.ComponentModel.DataAnnotations;

namespace PimVendas.Models
{
    public class EmprestimosModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome do recebedor!")]
        public String Recebedor { get; set; }

        [Required(ErrorMessage = "Digite o nome do fornecedor!")]
        public String Fornecedor { get; set; }

        [Required(ErrorMessage = "Digite o nome do livro!")]
        public String LivroEmprestado { get; set; }

        public DateTime dataUltimaAtualizacao { get; set; } = DateTime.Now;
    }
}
