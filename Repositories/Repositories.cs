using LiveStore.Data;
using LiveStore.Models;
using LiveStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiveStore.Repositories
{
    public class LiveRepository : ILiveRepository
    {
        private readonly ApplicationDbContext _db;
        public LiveRepository(ApplicationDbContext db) => _db = db;

        public IEnumerable<LiveModel> ObterTodas() =>
            _db.Lives.Include(l => l.Vendas)
                     .OrderByDescending(l => l.IniciadaEm)
                     .ToList();

        public LiveModel? ObterPorId(int id) =>
            _db.Lives.Include(l => l.Vendas)
                     .FirstOrDefault(l => l.Id == id);

        public LiveModel? ObterAtiva() =>
            _db.Lives.Include(l => l.Vendas)
                     .FirstOrDefault(l => l.Status == StatusLive.Ativa);

        public void Adicionar(LiveModel live) => _db.Lives.Add(live);

        public void SalvarAlteracoes() => _db.SaveChanges();
    }

    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _db;
        public VendaRepository(ApplicationDbContext db) => _db = db;

        public IEnumerable<VendaModel> ObterPorLive(int liveId) =>
            _db.Vendas.Include(v => v.Cliente)
                      .Where(v => v.LiveId == liveId)
                      .OrderByDescending(v => v.DataVenda)
                      .ToList();

        public VendaModel? ObterPorId(int id) =>
            _db.Vendas.Include(v => v.Cliente)
                      .FirstOrDefault(v => v.Id == id);

        public bool ExisteCodigoNaLive(int liveId, string codigo, int? excetoVendaId)
        {
            var query = _db.Vendas.Where(v => v.LiveId == liveId && v.Codigo == codigo);
            if (excetoVendaId.HasValue)
                query = query.Where(v => v.Id != excetoVendaId.Value);
            return query.Any();
        }

        public void Adicionar(VendaModel venda) => _db.Vendas.Add(venda);

        public void Remover(VendaModel venda) => _db.Vendas.Remove(venda);

        public void SalvarAlteracoes() => _db.SaveChanges();
    }

    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _db;
        public ClienteRepository(ApplicationDbContext db) => _db = db;

        public IEnumerable<ClienteModel> ObterTodos() =>
            _db.Clientes.OrderBy(c => c.InstagramUser).ToList();

        public ClienteModel? ObterPorInstagram(string instagram) =>
            _db.Clientes.FirstOrDefault(c => c.InstagramUser == instagram);

        public void Adicionar(ClienteModel cliente) => _db.Clientes.Add(cliente);

        public void SalvarAlteracoes() => _db.SaveChanges();
    }

    public class GastoRepository : IGastoRepository
    {
        private readonly ApplicationDbContext _db;
        public GastoRepository(ApplicationDbContext db) => _db = db;

        public IEnumerable<GastoMensalModel> ObterTodos() =>
            _db.Gastos.OrderByDescending(g => g.Data).ToList();

        public GastoMensalModel? ObterPorId(int id) =>
            _db.Gastos.FirstOrDefault(g => g.Id == id);

        public void Adicionar(GastoMensalModel gasto) => _db.Gastos.Add(gasto);

        public void Remover(GastoMensalModel gasto) => _db.Gastos.Remove(gasto);

        public void SalvarAlteracoes() => _db.SaveChanges();
    }
}
