using PimVendas.Models;

namespace PimVendas.Services
{
    public interface IVendaService
    {
        IEnumerable<VendaModel> ObterTodasVendas();
        VendaModel? ObterVendaPorId(int id);
        void CadastrarVenda(VendaModel venda);
        bool EditarVenda(VendaModel venda);
        bool ExcluirVenda(int id);

        // Dados para o Dashboard
        DashboardViewModel ObterDadosDashboard();
    }

    public class DashboardViewModel
    {
        public int TotalVendas { get; set; }
        public int VendasHoje { get; set; }
        public decimal FaturamentoTotal { get; set; }
        public decimal FaturamentoHoje { get; set; }
        public int VendasPendentes { get; set; }
        public int VendasPagas { get; set; }
        public int VendasCanceladas { get; set; }
        public int VendasEntregues { get; set; }
        public List<VendaModel> UltimasVendas { get; set; } = new();
    }
}
