using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class LiveController : Controller
    {
        private readonly ILiveService  _liveService;
        private readonly IVendaService _vendaService;

        public LiveController(ILiveService liveService, IVendaService vendaService)
        {
            _liveService  = liveService;
            _vendaService = vendaService;
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
    }
}
