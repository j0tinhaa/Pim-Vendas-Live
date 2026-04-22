using PimVendas.Models;
using PimVendas.Repositories;

namespace PimVendas.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _repository;

        public VendaService(IVendaRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<VendaModel> ObterTodasVendas()
        {
            return _repository.ObterTodas();
        }

        public VendaModel? ObterVendaPorId(int id)
        {
            return _repository.ObterPorId(id);
        }

        public void CadastrarVenda(VendaModel venda)
        {
            venda.DataVenda = DateTime.Now;
            venda.DataAtualizacao = DateTime.Now;
            _repository.Adicionar(venda);
            _repository.SalvarAlteracoes();
        }

        public bool EditarVenda(VendaModel venda)
        {
            var existente = _repository.ObterPorId(venda.Id);
            if (existente == null) return false;

            // Preserva a data original da venda; atualiza apenas a data de atualização
            venda.DataVenda = existente.DataVenda;
            venda.DataAtualizacao = DateTime.Now;

            _repository.Atualizar(venda);
            _repository.SalvarAlteracoes();
            return true;
        }

        public bool ExcluirVenda(int id)
        {
            var venda = _repository.ObterPorId(id);
            if (venda == null) return false;

            _repository.Remover(venda);
            _repository.SalvarAlteracoes();
            return true;
        }

        public DashboardViewModel ObterDadosDashboard()
        {
            var todas = _repository.ObterTodas().ToList();
            var hoje = DateTime.Today;

            return new DashboardViewModel
            {
                TotalVendas        = todas.Count,
                VendasHoje         = todas.Count(v => v.DataVenda.Date == hoje),
                FaturamentoTotal   = todas.Where(v => v.Status != StatusVenda.Cancelado).Sum(v => v.Valor),
                FaturamentoHoje    = todas.Where(v => v.DataVenda.Date == hoje && v.Status != StatusVenda.Cancelado).Sum(v => v.Valor),
                VendasPendentes    = todas.Count(v => v.Status == StatusVenda.Pendente),
                VendasPagas        = todas.Count(v => v.Status == StatusVenda.Pago),
                VendasCanceladas   = todas.Count(v => v.Status == StatusVenda.Cancelado),
                VendasEntregues    = todas.Count(v => v.Status == StatusVenda.Entregue),
                UltimasVendas      = todas.Take(5).ToList()
            };
        }
    }
}
