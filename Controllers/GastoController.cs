using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class GastoController : Controller
    {
        private readonly IGastoService _service;

        public GastoController(IGastoService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(_service.ObterTodos());
        }

        [HttpGet]
        public IActionResult Cadastrar() => View(new GastoMensalModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Cadastrar(GastoMensalModel gasto)
        {
            if (!ModelState.IsValid) return View(gasto);

            _service.Cadastrar(gasto);
            TempData["MensagemSucesso"] = "Gasto registrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var g = _service.ObterPorId(id);
            if (g == null) return NotFound();
            return View(g);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(GastoMensalModel gasto)
        {
            if (!ModelState.IsValid) return View(gasto);

            _service.Editar(gasto);
            TempData["MensagemSucesso"] = "Gasto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            _service.Excluir(id);
            TempData["MensagemSucesso"] = "Gasto removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
