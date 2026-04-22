using PimVendas.Models;

namespace PimVendas.Repositories
{
    public interface IVendaRepository
    {
        IEnumerable<VendaModel> ObterTodas();
        VendaModel? ObterPorId(int id);
        void Adicionar(VendaModel venda);
        void Atualizar(VendaModel venda);
        void Remover(VendaModel venda);
        void SalvarAlteracoes();
    }
}
