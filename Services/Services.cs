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

        public DashboardService(ILiveRepository liveRepo,
                                IVendaRepository vendaRepo,
                                IClienteRepository clienteRepo)
        {
            _liveRepo    = liveRepo;
            _vendaRepo   = vendaRepo;
            _clienteRepo = clienteRepo;
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
                UltimasVendas        = todasVendas.OrderByDescending(v => v.DataVenda).Take(6).ToList()
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
        private readonly IProdutoRepository _produtoRepo;

        public VendaService(IVendaRepository vendaRepo,
                            IClienteRepository clienteRepo,
                            IProdutoRepository produtoRepo)
        {
            _vendaRepo   = vendaRepo;
            _clienteRepo = clienteRepo;
            _produtoRepo = produtoRepo;
        }

        public IEnumerable<VendaModel> ObterPorLive(int liveId) =>
            _vendaRepo.ObterPorLive(liveId);

        public VendaModel? ObterPorId(int id) => _vendaRepo.ObterPorId(id);

        public ResultadoVenda RegistrarVenda(NovaVendaInput input)
        {
            bool clienteCriado = false;

            // Cadastro automático de cliente
            var instagram = input.ClienteInstagram.Trim().ToLower();
            var cliente   = _clienteRepo.ObterPorInstagram(instagram);
            if (cliente == null)
            {
                cliente = new ClienteModel { InstagramUser = instagram };
                _clienteRepo.Adicionar(cliente);
                _clienteRepo.SalvarAlteracoes();
                clienteCriado = true;
            }

            // Busca ou cria produto automaticamente pelo código
            var codigoProduto = input.CodigoProduto.Trim().ToUpper();
            string nomeProduto = input.NomeProduto.Trim();
            decimal valor      = input.Valor;

            var produto = _produtoRepo.ObterPorCodigo(codigoProduto);
            if (produto == null)
            {
                // Cadastro automático: cria com os dados informados na venda
                produto = new ProdutoModel
                {
                    Codigo   = codigoProduto,
                    Nome     = string.IsNullOrWhiteSpace(nomeProduto) ? codigoProduto : nomeProduto,
                    Preco    = valor,
                    Ativo    = true,
                    CriadoEm = DateTime.Now
                };
                _produtoRepo.Adicionar(produto);
                _produtoRepo.SalvarAlteracoes();
            }
            else
            {
                // Produto encontrado: preenche nome/valor se não informados
                if (string.IsNullOrWhiteSpace(nomeProduto)) nomeProduto = produto.Nome;
                if (valor <= 0) valor = produto.Preco;
            }

            int? produtoId = produto.Id;

            var venda = new VendaModel
            {
                LiveId           = input.LiveId,
                ClienteInstagram = instagram,
                ProdutoId        = produtoId,
                CodigoProduto    = codigoProduto,
                NomeProduto      = nomeProduto,
                Valor            = valor,
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

        public bool EditarVenda(int id, EditarVendaInput input)
        {
            var venda = _vendaRepo.ObterPorId(id);
            if (venda == null) return false;

            var instagram = input.ClienteInstagram.Trim().ToLower();
            var cliente   = _clienteRepo.ObterPorInstagram(instagram);
            if (cliente == null)
            {
                cliente = new ClienteModel { InstagramUser = instagram };
                _clienteRepo.Adicionar(cliente);
                _clienteRepo.SalvarAlteracoes();
            }

            var produto = _produtoRepo.ObterPorCodigo(input.CodigoProduto.Trim().ToUpper());

            venda.ClienteInstagram = instagram;
            venda.ProdutoId        = produto?.Id;
            venda.CodigoProduto    = input.CodigoProduto.Trim().ToUpper();
            venda.NomeProduto      = input.NomeProduto.Trim();
            venda.Valor            = input.Valor;
            venda.Status           = input.Status;
            venda.Observacoes      = input.Observacoes?.Trim();
            venda.DataAtualizacao  = DateTime.Now;

            _vendaRepo.SalvarAlteracoes();
            return true;
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

    // ── Produto ──────────────────────────────────────────────────────────────
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repo;
        public ProdutoService(IProdutoRepository repo) => _repo = repo;

        public IEnumerable<ProdutoModel> ObterTodos()           => _repo.ObterTodos();
        public ProdutoModel? ObterPorId(int id)                 => _repo.ObterPorId(id);
        public ProdutoModel? ObterPorCodigo(string codigo)      => _repo.ObterPorCodigo(codigo);

        public void Cadastrar(ProdutoModel produto)
        {
            produto.Codigo  = produto.Codigo.Trim().ToUpper();
            produto.CriadoEm = DateTime.Now;
            _repo.Adicionar(produto);
            _repo.SalvarAlteracoes();
        }

        public bool Editar(ProdutoModel produto)
        {
            var existente = _repo.ObterPorId(produto.Id);
            if (existente == null) return false;

            existente.Codigo   = produto.Codigo.Trim().ToUpper();
            existente.Nome     = produto.Nome.Trim();
            existente.Preco    = produto.Preco;
            existente.Tipo     = produto.Tipo?.Trim();
            existente.Cor      = produto.Cor?.Trim();
            existente.Tamanho  = produto.Tamanho?.Trim();
            existente.Ativo    = produto.Ativo;

            _repo.SalvarAlteracoes();
            return true;
        }

        public bool Excluir(int id)
        {
            var produto = _repo.ObterPorId(id);
            if (produto == null) return false;

            produto.Ativo = false; // soft delete: não exclui do banco
            _repo.SalvarAlteracoes();
            return true;
        }
    }

    // ── Cliente ──────────────────────────────────────────────────────────────
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        public ClienteService(IClienteRepository repo) => _repo = repo;

        public IEnumerable<ClienteModel> ObterTodos() => _repo.ObterTodos();

        public ClienteModel? ObterPorInstagram(string instagram) =>
            _repo.ObterPorInstagram(instagram.Trim().ToLower());

        public ClienteModel ObterOuCriar(string instagram)
        {
            var ig      = instagram.Trim().ToLower();
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
}
