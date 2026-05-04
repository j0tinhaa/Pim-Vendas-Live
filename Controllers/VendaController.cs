using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class VendaController : Controller
    {
        private readonly IVendaService    _vendaService;
        private readonly ILiveService     _liveService;
        private readonly IClienteService  _clienteService;

        public VendaController(IVendaService vendaService,
                               ILiveService liveService,
                               IClienteService clienteService)
        {
            _vendaService   = vendaService;
            _liveService    = liveService;
            _clienteService = clienteService;
        }

        // GET /Venda/Cadastrar?liveId=5
        [HttpGet]
        public IActionResult Cadastrar(int liveId)
        {
            var live = _liveService.ObterPorId(liveId);
            if (live == null) return NotFound();

            ViewBag.Live = live;
            return View(new NovaVendaInput { LiveId = liveId });
        }

        // POST /Venda/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(NovaVendaInput input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Live = _liveService.ObterPorId(input.LiveId);
                return View(input);
            }

            var resultado = _vendaService.RegistrarVenda(input);

            if (!resultado.Sucesso)
            {
                ModelState.AddModelError(nameof(input.Codigo), resultado.Erro ?? "Erro ao registrar venda.");
                ViewBag.Live = _liveService.ObterPorId(input.LiveId);
                return View(input);
            }

            if (resultado.ClienteCriado)
                TempData["MensagemInfo"] = $"Cliente @{input.ClienteInstagram} cadastrado automaticamente.";

            TempData["MensagemSucesso"] = "Venda registrada com sucesso!";
            return RedirectToAction("Ativa", "Live");
        }

        // GET /Venda/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var venda = _vendaService.ObterPorId(id);
            if (venda == null) return NotFound();

            var input = new EditarVendaInput
            {
                ClienteInstagram = venda.ClienteInstagram,
                Codigo           = venda.Codigo,
                Nome             = venda.Nome,
                Descricao        = venda.Descricao,
                Valor            = venda.Valor,
                Status           = venda.Status,
                Observacoes      = venda.Observacoes
            };

            ViewBag.VendaId = id;
            ViewBag.LiveId  = venda.LiveId;
            return View(input);
        }

        // POST /Venda/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, EditarVendaInput input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VendaId = id;
                return View(input);
            }

            var resultado = _vendaService.EditarVenda(id, input);
            if (!resultado.Sucesso)
            {
                ModelState.AddModelError(nameof(input.Codigo), resultado.Erro ?? "Erro ao editar venda.");
                ViewBag.VendaId = id;
                return View(input);
            }

            TempData["MensagemSucesso"] = "Venda atualizada!";

            // Retorna para a tela correta (live ativa ou detalhes)
            var venda = _vendaService.ObterPorId(id);
            var live  = venda != null ? _liveService.ObterPorId(venda.LiveId) : null;
            if (live?.Status == StatusLive.Ativa)
                return RedirectToAction("Ativa", "Live");

            return RedirectToAction("Detalhes", "Live", new { id = venda?.LiveId });
        }

        // POST /Venda/Excluir/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id, int liveId)
        {
            _vendaService.ExcluirVenda(id);
            TempData["MensagemSucesso"] = "Venda removida.";

            var live = _liveService.ObterPorId(liveId);
            if (live?.Status == StatusLive.Ativa)
                return RedirectToAction("Ativa", "Live");

            return RedirectToAction("Detalhes", "Live", new { id = liveId });
        }

        // GET /Venda/BuscarClientes?term=abc (AJAX Autocomplete)
        [HttpGet]
        public IActionResult BuscarClientes(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new object[] {});
            term = term.Trim().ToLower();

            var clientes = _clienteService.ObterTodos()
                .Where(c => c.InstagramUser.Contains(term) || (c.Nome != null && c.Nome.ToLower().Contains(term)))
                .Select(c => new {
                    instagram = c.ClienteInstagramFormatado,
                    nome = c.Nome
                })
                .Take(10)
                .ToList();

            return Json(clientes);
        }
    }
}
