using LiveStore.Models;

namespace LiveStore.Repositories.Interfaces
{
    public interface ILiveRepository
    {
        IEnumerable<LiveModel> ObterTodas();
        LiveModel? ObterPorId(int id);
        LiveModel? ObterAtiva();
        void Adicionar(LiveModel live);
        void SalvarAlteracoes();
    }

    public interface IVendaRepository
    {
        IEnumerable<VendaModel> ObterPorLive(int liveId);
        VendaModel? ObterPorId(int id);
        bool ExisteCodigoNaLive(int liveId, string codigo, int? excetoVendaId);
        void Adicionar(VendaModel venda);
        void Remover(VendaModel venda);
        void SalvarAlteracoes();
    }

    public interface IClienteRepository
    {
        IEnumerable<ClienteModel> ObterTodos();
        ClienteModel? ObterPorInstagram(string instagram);
        void Adicionar(ClienteModel cliente);
        void SalvarAlteracoes();
    }

    public interface IGastoRepository
    {
        IEnumerable<GastoMensalModel> ObterTodos();
        GastoMensalModel? ObterPorId(int id);
        void Adicionar(GastoMensalModel gasto);
        void Remover(GastoMensalModel gasto);
        void SalvarAlteracoes();
    }
}
