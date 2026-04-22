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

            // Atualiza os campos no objeto já rastreado pelo EF
            existente.Cliente = venda.Cliente;
            existente.CodigoProduto = venda.CodigoProduto;
            existente.Produto = venda.Produto;
            existente.Valor = venda.Valor;
            existente.Status = venda.Status;
            existente.Observacoes = venda.Observacoes;
            existente.DataAtualizacao = DateTime.Now;
            // DataVenda preservada automaticamente (não sobrescrevemos)

            _repository.SalvarAlteracoes(); // sem chamar Atualizar() — o EF já está rastreando
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
