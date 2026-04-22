using Microsoft.AspNetCore.Mvc;
using PimVendas.Models;
using PimVendas.Services;

namespace PimVendas.Controllers
{
    public class VendaController : Controller
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        // GET: /Venda
        public IActionResult Index()
        {
            var vendas = _vendaService.ObterTodasVendas();
            return View(vendas);
        }

        // GET: /Venda/Cadastrar
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new VendaModel());
        }

        // POST: /Venda/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(VendaModel venda)
        {
            if (!ModelState.IsValid)
                return View(venda);

            _vendaService.CadastrarVenda(venda);
            TempData["MensagemSucesso"] = "Venda cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Venda/Editar/5
        [HttpGet]
        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var venda = _vendaService.ObterVendaPorId(id.Value);
            if (venda == null)
                return NotFound();

            return View(venda);
        }

        // POST: /Venda/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(VendaModel venda)
        {
            if (!ModelState.IsValid)
                return View(venda);

            var sucesso = _vendaService.EditarVenda(venda);
            if (!sucesso)
                return NotFound();

            TempData["MensagemSucesso"] = "Venda editada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Venda/Excluir/5
        [HttpGet]
        public IActionResult Excluir(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var venda = _vendaService.ObterVendaPorId(id.Value);
            if (venda == null)
                return NotFound();

            return View(venda);
        }

        // POST: /Venda/Excluir
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            var sucesso = _vendaService.ExcluirVenda(id);
            if (!sucesso)
                return NotFound();

            TempData["MensagemSucesso"] = "Venda excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
