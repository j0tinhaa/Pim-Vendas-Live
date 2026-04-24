using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStore.Models
{
    public class ClienteLiveRelatorioModel
    {
        [Required]
        public int LiveId { get; set; }

        [ForeignKey("LiveId")]
        public LiveModel? Live { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClienteInstagram { get; set; } = string.Empty;

        [ForeignKey("ClienteInstagram")]
        public ClienteModel? Cliente { get; set; }

        public bool Enviado { get; set; }

        public DateTime DataEnvio { get; set; }
    }
}
