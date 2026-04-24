using LiveStore.Models;

namespace LiveStore.Services.Interfaces
{
    public interface IDashboardService
    {
        DashboardViewModel ObterDados();
    }

    public interface ILiveService
    {
        IEnumerable<LiveModel> ObterTodas();
        LiveModel? ObterPorId(int id);
        LiveModel? ObterAtiva();
        LiveModel IniciarNovaLive(string nome, string? descricao);
        void EncerrarLive(int id);
    }

    public interface IVendaService
    {
        IEnumerable<VendaModel> ObterPorLive(int liveId);
        VendaModel? ObterPorId(int id);
        ResultadoVenda RegistrarVenda(NovaVendaInput input);
        bool EditarVenda(int id, EditarVendaInput input);
        bool ExcluirVenda(int id);
    }

    public interface IProdutoService
    {
        IEnumerable<ProdutoModel> ObterTodos();
        ProdutoModel? ObterPorId(int id);
        ProdutoModel? ObterPorCodigo(string codigo);
        void Cadastrar(ProdutoModel produto);
        bool Editar(ProdutoModel produto);
        bool Excluir(int id);
    }

    public interface IClienteService
    {
        IEnumerable<ClienteModel> ObterTodos();
        ClienteModel? ObterPorInstagram(string instagram);
        /// <summary>Retorna cliente existente ou cria automaticamente.</summary>
        ClienteModel ObterOuCriar(string instagram);
        bool Editar(ClienteModel cliente);
    }

    public interface IGastoService
    {
        IEnumerable<GastoMensalModel> ObterTodos();
        GastoMensalModel? ObterPorId(int id);
        void Cadastrar(GastoMensalModel gasto);
        bool Editar(GastoMensalModel gasto);
        bool Excluir(int id);
    }

    public interface IRelatorioService
    {
        byte[] GerarPdfVendasCliente(ClienteModel cliente, List<VendaModel> vendas, LiveModel live);
    }

    /// <summary>
    /// Serviço de envio de relatório via WhatsApp.
    /// Implemente MockWhatsAppService para dev/local,
    /// ou configure TwilioWhatsAppService / CloudApiWhatsAppService para produção.
    /// </summary>
    public interface IWhatsAppService
    {
        /// <returns>Resultado com sucesso, mensagem de status e URL de download do PDF.</returns>
        Task<ResultadoEnvioRelatorio> EnviarRelatorioAsync(
            string telefone,
            string nomeCliente,
            byte[] pdfBytes,
            string nomeArquivoPdf);
    }

    public class ResultadoEnvioRelatorio
    {
        public bool    Sucesso      { get; set; }
        public string  Mensagem     { get; set; } = string.Empty;
        /// <summary>URL pública do PDF (usada para envio via API de WhatsApp).</summary>
        public string? UrlPdf       { get; set; }
        /// <summary>Link wa.me para abrir WhatsApp Web (fallback).</summary>
        public string? UrlWhatsApp  { get; set; }
    }

    // ── ViewModels / Inputs ──────────────────────────────────────────────────

    public class DashboardViewModel
    {
        public int    TotalLives           { get; set; }
        public int    TotalVendas          { get; set; }
        public int    TotalClientes        { get; set; }
        public decimal FaturamentoTotal    { get; set; }
        public decimal FaturamentoHoje     { get; set; }
        public int    VendasHoje           { get; set; }
        public LiveModel? LiveMaiorFaturamento { get; set; }
        public LiveModel? LiveAtiva        { get; set; }
        public List<VendaModel> UltimasVendas { get; set; } = new();

        // Novas Métricas
        public TimeSpan? MelhorHorarioLive { get; set; }
        public decimal TicketMedio         { get; set; }
        public decimal TotalGastosMes      { get; set; }
        public decimal LucroLiquidoMes     { get; set; }
    }

    public class NovaVendaInput
    {
        public int    LiveId          { get; set; }
        public string ClienteInstagram { get; set; } = string.Empty;
        public string CodigoProduto   { get; set; } = string.Empty;
        public string NomeProduto     { get; set; } = string.Empty;
        public decimal Valor          { get; set; }
        public StatusVenda Status     { get; set; } = StatusVenda.Reservado;
        public string? Observacoes    { get; set; }
    }

    public class EditarVendaInput
    {
        public string ClienteInstagram { get; set; } = string.Empty;
        public string CodigoProduto   { get; set; } = string.Empty;
        public string NomeProduto     { get; set; } = string.Empty;
        public decimal Valor          { get; set; }
        public StatusVenda Status     { get; set; }
        public string? Observacoes    { get; set; }
    }

    public class ResultadoVenda
    {
        public bool Sucesso          { get; set; }
        public string? Erro          { get; set; }
        public bool ClienteCriado    { get; set; }
        public VendaModel? Venda     { get; set; }
    }

    // ── ViewModel para tela de envio de relatório ────────────────────────────

    public class ClienteRelatorioItem
    {
        public ClienteModel Cliente       { get; set; } = null!;
        public List<VendaModel> Vendas    { get; set; } = new();
        public bool TemTelefone           => !string.IsNullOrWhiteSpace(Cliente.Telefone);
        public bool RelatorioEnviado      { get; set; } = false;
    }

    public class RelatorioEnvioViewModel
    {
        public LiveModel Live                      { get; set; } = null!;
        public List<ClienteRelatorioItem> Clientes { get; set; } = new();
    }
}
