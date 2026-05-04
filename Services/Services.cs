using LiveStore.Models;
using LiveStore.Repositories.Interfaces;
using LiveStore.Services.Interfaces;

namespace LiveStore.Services
{
    // ── Dashboard ────────────────────────────────────────────────────────────
    public class DashboardService : IDashboardService
    {
        private readonly ILiveRepository    _liveRepo;
        private readonly IVendaRepository   _vendaRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IGastoRepository   _gastoRepo;

        public DashboardService(ILiveRepository liveRepo,
                                IVendaRepository vendaRepo,
                                IClienteRepository clienteRepo,
                                IGastoRepository gastoRepo)
        {
            _liveRepo    = liveRepo;
            _vendaRepo   = vendaRepo;
            _clienteRepo = clienteRepo;
            _gastoRepo   = gastoRepo;
        }

        public DashboardViewModel ObterDados()
        {
            var lives   = _liveRepo.ObterTodas().ToList();
            var clientes = _clienteRepo.ObterTodos().ToList();
            var hoje    = DateTime.Today;

            // Todas as vendas de todas as lives
            var todasVendas = lives.SelectMany(l => l.Vendas).ToList();
            var vendasAtivas = todasVendas.Where(v => v.Status != StatusVenda.Cancelado).ToList();

            var liveMaiorFat = lives.OrderByDescending(l => l.FaturamentoTotal).FirstOrDefault();

            // Métricas Inteligentes
            decimal faturamentoMes = vendasAtivas.Where(v => v.DataVenda.Year == hoje.Year && v.DataVenda.Month == hoje.Month).Sum(v => v.Valor);
            var gastosMes = _gastoRepo.ObterTodos().Where(g => g.Data.Year == hoje.Year && g.Data.Month == hoje.Month).Sum(g => g.Valor);

            decimal ticketMedio = vendasAtivas.Count > 0 ? vendasAtivas.Average(v => v.Valor) : 0;

            TimeSpan? melhorHorario = null;
            if (vendasAtivas.Any())
            {
                // Agrupa as vendas pela hora do dia e pega a hora com maior número de vendas (ou faturamento)
                var horaMaisVendas = vendasAtivas
                    .GroupBy(v => v.DataVenda.Hour)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();
                melhorHorario = new TimeSpan(horaMaisVendas, 0, 0);
            }

            return new DashboardViewModel
            {
                TotalLives           = lives.Count,
                TotalVendas          = todasVendas.Count,
                TotalClientes        = clientes.Count,
                FaturamentoTotal     = vendasAtivas.Sum(v => v.Valor),
                FaturamentoHoje      = vendasAtivas.Where(v => v.DataVenda.Date == hoje).Sum(v => v.Valor),
                VendasHoje           = todasVendas.Count(v => v.DataVenda.Date == hoje),
                LiveMaiorFaturamento = liveMaiorFat,
                LiveAtiva            = _liveRepo.ObterAtiva(),
                UltimasVendas        = todasVendas.OrderByDescending(v => v.DataVenda).Take(6).ToList(),

                TicketMedio       = ticketMedio,
                TotalGastosMes    = gastosMes,
                LucroLiquidoMes   = faturamentoMes - gastosMes,
                MelhorHorarioLive = melhorHorario
            };
        }
    }

    // ── Live ─────────────────────────────────────────────────────────────────
    public class LiveService : ILiveService
    {
        private readonly ILiveRepository _repo;
        public LiveService(ILiveRepository repo) => _repo = repo;

        public IEnumerable<LiveModel> ObterTodas() => _repo.ObterTodas();
        public LiveModel? ObterPorId(int id)       => _repo.ObterPorId(id);
        public LiveModel? ObterAtiva()             => _repo.ObterAtiva();

        public LiveModel IniciarNovaLive(string nome, string? descricao)
        {
            // Encerra qualquer live ativa antes de criar nova
            var ativa = _repo.ObterAtiva();
            if (ativa != null)
            {
                ativa.Status      = StatusLive.Encerrada;
                ativa.EncerradaEm = DateTime.Now;
                _repo.SalvarAlteracoes();
            }

            var novaLive = new LiveModel
            {
                Nome      = nome.Trim(),
                Descricao = descricao?.Trim(),
                Status    = StatusLive.Ativa,
                IniciadaEm = DateTime.Now
            };

            _repo.Adicionar(novaLive);
            _repo.SalvarAlteracoes();
            return novaLive;
        }

        public void EncerrarLive(int id)
        {
            var live = _repo.ObterPorId(id);
            if (live == null) return;

            live.Status      = StatusLive.Encerrada;
            live.EncerradaEm = DateTime.Now;
            _repo.SalvarAlteracoes();
        }
    }

    // ── Venda ────────────────────────────────────────────────────────────────
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository   _vendaRepo;
        private readonly IClienteRepository _clienteRepo;

        public VendaService(IVendaRepository vendaRepo,
                            IClienteRepository clienteRepo)
        {
            _vendaRepo   = vendaRepo;
            _clienteRepo = clienteRepo;
        }

        public IEnumerable<VendaModel> ObterPorLive(int liveId) =>
            _vendaRepo.ObterPorLive(liveId);

        public VendaModel? ObterPorId(int id) => _vendaRepo.ObterPorId(id);

        public bool ExisteCodigoNaLive(int liveId, string codigo, int? excetoVendaId = null)
        {
            var cod = (codigo ?? string.Empty).Trim().ToUpper();
            return _vendaRepo.ExisteCodigoNaLive(liveId, cod, excetoVendaId);
        }

        public ResultadoVenda RegistrarVenda(NovaVendaInput input)
        {
            var codigo = input.Codigo.Trim().ToUpper();

            if (_vendaRepo.ExisteCodigoNaLive(input.LiveId, codigo, null))
            {
                return new ResultadoVenda
                {
                    Sucesso = false,
                    Erro    = "Código já usado nesta live."
                };
            }

            bool clienteCriado = false;

            // Cadastro automático de cliente
            var instagram = input.ClienteInstagram.Trim().ToLower();
            if (!instagram.StartsWith("@")) instagram = "@" + instagram;
            var cliente   = _clienteRepo.ObterPorInstagram(instagram);
            if (cliente == null)
            {
                cliente = new ClienteModel { InstagramUser = instagram };
                _clienteRepo.Adicionar(cliente);
                _clienteRepo.SalvarAlteracoes();
                clienteCriado = true;
            }

            var venda = new VendaModel
            {
                LiveId           = input.LiveId,
                ClienteInstagram = instagram,
                Codigo           = codigo,
                Nome             = input.Nome.Trim(),
                Descricao        = input.Descricao?.Trim(),
                Valor            = input.Valor,
                Status           = input.Status,
                Observacoes      = input.Observacoes?.Trim(),
                DataVenda        = DateTime.Now,
                DataAtualizacao  = DateTime.Now
            };

            _vendaRepo.Adicionar(venda);
            _vendaRepo.SalvarAlteracoes();

            return new ResultadoVenda
            {
                Sucesso       = true,
                ClienteCriado = clienteCriado,
                Venda         = venda
            };
        }

        public ResultadoVenda EditarVenda(int id, EditarVendaInput input)
        {
            var venda = _vendaRepo.ObterPorId(id);
            if (venda == null) return new ResultadoVenda { Sucesso = false, Erro = "Venda não encontrada." };

            var codigo = input.Codigo.Trim().ToUpper();

            if (_vendaRepo.ExisteCodigoNaLive(venda.LiveId, codigo, id))
            {
                return new ResultadoVenda
                {
                    Sucesso = false,
                    Erro    = "Código já usado nesta live."
                };
            }

            var instagram = input.ClienteInstagram.Trim().ToLower();
            if (!instagram.StartsWith("@")) instagram = "@" + instagram;
            var cliente   = _clienteRepo.ObterPorInstagram(instagram);
            if (cliente == null)
            {
                cliente = new ClienteModel { InstagramUser = instagram };
                _clienteRepo.Adicionar(cliente);
                _clienteRepo.SalvarAlteracoes();
            }

            venda.ClienteInstagram = instagram;
            venda.Codigo           = codigo;
            venda.Nome             = input.Nome.Trim();
            venda.Descricao        = input.Descricao?.Trim();
            venda.Valor            = input.Valor;
            venda.Status           = input.Status;
            venda.Observacoes      = input.Observacoes?.Trim();
            venda.DataAtualizacao  = DateTime.Now;

            _vendaRepo.SalvarAlteracoes();
            return new ResultadoVenda { Sucesso = true, Venda = venda };
        }

        public bool ExcluirVenda(int id)
        {
            var venda = _vendaRepo.ObterPorId(id);
            if (venda == null) return false;

            _vendaRepo.Remover(venda);
            _vendaRepo.SalvarAlteracoes();
            return true;
        }
    }

    // ── Cliente ──────────────────────────────────────────────────────────────
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        public ClienteService(IClienteRepository repo) => _repo = repo;

        public IEnumerable<ClienteModel> ObterTodos() => _repo.ObterTodos();

        private string NormalizarInstagram(string instagram)
        {
            if (string.IsNullOrWhiteSpace(instagram)) return string.Empty;
            var norm = instagram.Trim().ToLower();
            if (!norm.StartsWith("@")) norm = "@" + norm;
            return norm;
        }

        public ClienteModel? ObterPorInstagram(string instagram) =>
            _repo.ObterPorInstagram(NormalizarInstagram(instagram));

        public ClienteModel ObterOuCriar(string instagram)
        {
            var ig      = NormalizarInstagram(instagram);
            var cliente = _repo.ObterPorInstagram(ig);
            if (cliente != null) return cliente;

            cliente = new ClienteModel { InstagramUser = ig };
            _repo.Adicionar(cliente);
            _repo.SalvarAlteracoes();
            return cliente;
        }

        public bool Editar(ClienteModel cliente)
        {
            var existente = _repo.ObterPorInstagram(cliente.InstagramUser);
            if (existente == null) return false;

            existente.Nome     = cliente.Nome?.Trim();
            existente.Telefone = cliente.Telefone?.Trim();
            _repo.SalvarAlteracoes();
            return true;
        }
    }

    // ── Gasto Mensal ─────────────────────────────────────────────────────────
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _repo;
        public GastoService(IGastoRepository repo) => _repo = repo;

        public IEnumerable<GastoMensalModel> ObterTodos() => _repo.ObterTodos();
        public GastoMensalModel? ObterPorId(int id) => _repo.ObterPorId(id);

        public void Cadastrar(GastoMensalModel gasto)
        {
            gasto.Descricao = gasto.Descricao.Trim();
            _repo.Adicionar(gasto);
            _repo.SalvarAlteracoes();
        }

        public bool Editar(GastoMensalModel gasto)
        {
            var existente = _repo.ObterPorId(gasto.Id);
            if (existente == null) return false;

            existente.Descricao = gasto.Descricao.Trim();
            existente.Valor     = gasto.Valor;
            existente.Data      = gasto.Data;

            _repo.SalvarAlteracoes();
            return true;
        }

        public bool Excluir(int id)
        {
            var gasto = _repo.ObterPorId(id);
            if (gasto == null) return false;

            _repo.Remover(gasto);
            _repo.SalvarAlteracoes();
            return true;
        }
    }
}
