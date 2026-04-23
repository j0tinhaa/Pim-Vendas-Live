using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class VendaController : Controller
    {
        private readonly IVendaService  _vendaService;
        private readonly ILiveService   _liveService;
        private readonly IProdutoService _produtoService;

        public VendaController(IVendaService vendaService,
                               ILiveService liveService,
                               IProdutoService produtoService)
        {
            _vendaService   = vendaService;
            _liveService    = liveService;
            _produtoService = produtoService;
        }

        // GET /Venda/Cadastrar?liveId=5
        [HttpGet]
        public IActionResult Cadastrar(int liveId)
        {
            var live = _liveService.ObterPorId(liveId);
            if (live == null) return NotFound();

            ViewBag.Live     = live;
            ViewBag.Produtos = _produtoService.ObterTodos().Where(p => p.Ativo);
            return View(new NovaVendaInput { LiveId = liveId });
        }

        // POST /Venda/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(NovaVendaInput input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Live     = _liveService.ObterPorId(input.LiveId);
                ViewBag.Produtos = _produtoService.ObterTodos().Where(p => p.Ativo);
                return View(input);
            }

            var resultado = _vendaService.RegistrarVenda(input);

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

            ViewBag.Produtos = _produtoService.ObterTodos().Where(p => p.Ativo);

            var input = new EditarVendaInput
            {
                ClienteInstagram = venda.ClienteInstagram,
                CodigoProduto    = venda.CodigoProduto,
                NomeProduto      = venda.NomeProduto,
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
                ViewBag.Produtos = _produtoService.ObterTodos().Where(p => p.Ativo);
                ViewBag.VendaId  = id;
                return View(input);
            }

            var sucesso = _vendaService.EditarVenda(id, input);
            if (!sucesso) return NotFound();

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

        // GET /Venda/BuscarProduto?codigo=ROSA-001  (AJAX)
        [HttpGet]
        public IActionResult BuscarProduto(string codigo)
        {
            var produto = _produtoService.ObterPorCodigo(codigo.ToUpper());
            if (produto == null) return NotFound();

            return Json(new
            {
                id    = produto.Id,
                nome  = produto.Nome,
                preco = produto.Preco
            });
        }
    }
}
