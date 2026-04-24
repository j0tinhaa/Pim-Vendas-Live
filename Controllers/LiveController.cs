using LiveStore.Data;
using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class LiveController : Controller
    {
        private readonly ILiveService      _liveService;
        private readonly IVendaService     _vendaService;
        private readonly IClienteService   _clienteService;
        private readonly IRelatorioService  _relatorioService;
        private readonly IWhatsAppService   _whatsAppService;
        private readonly ApplicationDbContext _context;

        public LiveController(ILiveService liveService,
                              IVendaService vendaService,
                              IClienteService clienteService,
                              IRelatorioService relatorioService,
                              IWhatsAppService whatsAppService,
                              ApplicationDbContext context)
        {
            _liveService       = liveService;
            _vendaService      = vendaService;
            _clienteService    = clienteService;
            _relatorioService  = relatorioService;
            _whatsAppService   = whatsAppService;
            _context           = context;
        }

        // GET /Live — histórico de lives
        public IActionResult Index()
        {
            var lives = _liveService.ObterTodas();
            return View(lives);
        }

        // GET /Live/Nova
        [HttpGet]
        public IActionResult Nova()
        {
            return View();
        }

        // POST /Live/Nova
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nova(string nome, string? descricao)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                ModelState.AddModelError("nome", "Informe o nome da live.");
                return View();
            }

            var live = _liveService.IniciarNovaLive(nome, descricao);
            TempData["MensagemSucesso"] = $"Live \"{live.Nome}\" iniciada!";
            return RedirectToAction("Ativa");
        }

        // GET /Live/Ativa — tela de vendas da live em curso
        public IActionResult Ativa()
        {
            var live = _liveService.ObterAtiva();
            if (live == null)
            {
                TempData["MensagemInfo"] = "Nenhuma live ativa. Inicie uma nova live.";
                return RedirectToAction(nameof(Nova));
            }

            var vendas = _vendaService.ObterPorLive(live.Id);
            ViewBag.Live   = live;
            ViewBag.LiveId = live.Id;
            return View(vendas);
        }

        // GET /Live/Detalhes/5 — vendas de uma live encerrada
        public IActionResult Detalhes(int id)
        {
            var live = _liveService.ObterPorId(id);
            if (live == null) return NotFound();

            var vendas = _vendaService.ObterPorLive(id);
            ViewBag.Live = live;
            return View(vendas);
        }

        // POST /Live/Encerrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Encerrar(int id)
        {
            _liveService.EncerrarLive(id);
            TempData["MensagemSucesso"] = "Live encerrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET /Live/EnviarRelatorio/5 — tela de seleção de clientes
        [HttpGet]
        public IActionResult EnviarRelatorio(int id)
        {
            var live = _liveService.ObterPorId(id);
            if (live == null) return NotFound();

            var vendas = _vendaService.ObterPorLive(id).ToList();

            // Busca relatórios já enviados nesta live
            var enviados = _context.RelatoriosEnviados
                .Where(r => r.LiveId == id && r.Enviado)
                .Select(r => r.ClienteInstagram)
                .ToHashSet();

            // Agrupa vendas por cliente
            var clientesDict = vendas
                .GroupBy(v => v.ClienteInstagram)
                .Select(g =>
                {
                    var cliente = _clienteService.ObterPorInstagram(g.Key)
                                  ?? new LiveStore.Models.ClienteModel { InstagramUser = g.Key };
                    return new ClienteRelatorioItem
                    {
                        Cliente = cliente,
                        Vendas  = g.ToList(),
                        RelatorioEnviado = enviados.Contains(g.Key)
                    };
                })
                .OrderBy(x => x.Cliente.Nome ?? x.Cliente.InstagramUser)
                .ToList();

            var vm = new RelatorioEnvioViewModel
            {
                Live     = live,
                Clientes = clientesDict
            };

            return View(vm);
        }

        // GET /Live/BaixarPdfCliente?liveId=5&instagram=@carla
        [HttpGet]
        public IActionResult BaixarPdfCliente(int liveId, string instagram)
        {
            var live = _liveService.ObterPorId(liveId);
            if (live == null) return NotFound();

            var cliente = _clienteService.ObterPorInstagram(instagram);
            if (cliente == null) return NotFound("Cliente não encontrado.");

            // Busca TODAS as vendas da live e filtra por cliente em memória
            // (Inclui navegação Produto já carregada pelo VendaRepository)
            var vendas = _vendaService.ObterPorLive(liveId)
                .Where(v => string.Equals(v.ClienteInstagram, instagram, StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.DataVenda)
                .ToList();

            var pdf = _relatorioService.GerarPdfVendasCliente(cliente, vendas, live);
            var nomeArq = $"relatorio_{instagram.Replace("@@","").Replace(" ","_")}_{live.IniciadaEm:yyyyMMdd}.pdf";
            return File(pdf, "application/pdf", nomeArq);
        }

        // POST /Live/EnviarRelatorioCliente — chamado via AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarRelatorioCliente([FromBody] EnvioClienteInput input)
        {
            if (input == null || input.LiveId <= 0 || string.IsNullOrWhiteSpace(input.Instagram))
                return BadRequest(new { sucesso = false, mensagem = "Dados inválidos." });

            var live = _liveService.ObterPorId(input.LiveId);
            if (live == null) return NotFound(new { sucesso = false, mensagem = "Live não encontrada." });

            var cliente = _clienteService.ObterPorInstagram(input.Instagram);
            if (cliente == null)
                return NotFound(new { sucesso = false, mensagem = "Cliente não encontrado." });

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
                return Ok(new { sucesso = false, mensagem = "Cliente sem telefone cadastrado." });

            var vendas = _vendaService.ObterPorLive(input.LiveId)
                .Where(v => string.Equals(v.ClienteInstagram, input.Instagram, StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.DataVenda)
                .ToList();

            var nomeCli = !string.IsNullOrWhiteSpace(cliente.Nome) ? cliente.Nome : cliente.InstagramUser;
            var nomeArq = $"relatorio_{input.Instagram.Replace("@@","").Replace(" ","_")}_{live.IniciadaEm:yyyyMMdd}.pdf";
            var pdf = _relatorioService.GerarPdfVendasCliente(cliente, vendas, live);

            var resultado = await _whatsAppService.EnviarRelatorioAsync(
                cliente.Telefone, nomeCli, pdf, nomeArq);

            if (resultado.Sucesso)
            {
                var registro = _context.RelatoriosEnviados
                    .FirstOrDefault(r => r.LiveId == live.Id && r.ClienteInstagram == cliente.InstagramUser);
                
                if (registro == null)
                {
                    registro = new ClienteLiveRelatorioModel
                    {
                        LiveId = live.Id,
                        ClienteInstagram = cliente.InstagramUser
                    };
                    _context.RelatoriosEnviados.Add(registro);
                }
                registro.Enviado = true;
                registro.DataEnvio = DateTime.Now;
                _context.SaveChanges();
            }

            return Ok(new
            {
                sucesso      = resultado.Sucesso,
                mensagem     = resultado.Mensagem,
                urlPdf       = resultado.UrlPdf,
                urlWhatsApp  = resultado.UrlWhatsApp
            });
        }
    }
}

// ViewModel para o corpo do POST AJAX
public class EnvioClienteInput
{
    public int    LiveId    { get; set; }
    public string Instagram { get; set; } = string.Empty;
}
