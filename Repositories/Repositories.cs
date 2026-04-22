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
                      .Include(v => v.Produto)
                      .Where(v => v.LiveId == liveId)
                      .OrderByDescending(v => v.DataVenda)
                      .ToList();

        public VendaModel? ObterPorId(int id) =>
            _db.Vendas.Include(v => v.Cliente)
                      .Include(v => v.Produto)
                      .FirstOrDefault(v => v.Id == id);

        public void Adicionar(VendaModel venda) => _db.Vendas.Add(venda);

        public void SalvarAlteracoes() => _db.SaveChanges();
    }

    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _db;
        public ProdutoRepository(ApplicationDbContext db) => _db = db;

        public IEnumerable<ProdutoModel> ObterTodos() =>
            _db.Produtos.OrderBy(p => p.Codigo).ToList();

        public ProdutoModel? ObterPorId(int id) =>
            _db.Produtos.FirstOrDefault(p => p.Id == id);

        public ProdutoModel? ObterPorCodigo(string codigo) =>
            _db.Produtos.FirstOrDefault(p => p.Codigo == codigo);

        public void Adicionar(ProdutoModel produto) => _db.Produtos.Add(produto);

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
}
