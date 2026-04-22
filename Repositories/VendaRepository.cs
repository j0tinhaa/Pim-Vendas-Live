using PimVendas.Data;
using PimVendas.Models;

namespace PimVendas.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _db;

        public VendaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<VendaModel> ObterTodas()
        {
            return _db.Vendas.OrderByDescending(v => v.DataVenda).ToList();
        }

        public VendaModel? ObterPorId(int id)
        {
            return _db.Vendas.FirstOrDefault(v => v.Id == id);
        }

        public void Adicionar(VendaModel venda)
        {
            _db.Vendas.Add(venda);
        }

        public void Atualizar(VendaModel venda)
        {
            _db.Vendas.Update(venda);
        }

        public void Remover(VendaModel venda)
        {
            _db.Vendas.Remove(venda);
        }

        public void SalvarAlteracoes()
        {
            _db.SaveChanges();
        }
    }
}
