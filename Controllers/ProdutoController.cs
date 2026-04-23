using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        private readonly IProdutoService _service;
        public ProdutoController(IProdutoService service) => _service = service;

        public IActionResult Index()
        {
            return View(_service.ObterTodos());
        }

        [HttpGet]
        public IActionResult Cadastrar() => View(new ProdutoModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Cadastrar(ProdutoModel produto)
        {
            if (!ModelState.IsValid) return View(produto);

            if (_service.ObterPorCodigo(produto.Codigo.ToUpper()) != null)
            {
                ModelState.AddModelError("Codigo", "Já existe um produto com esse código.");
                return View(produto);
            }

            _service.Cadastrar(produto);
            TempData["MensagemSucesso"] = "Produto cadastrado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var p = _service.ObterPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(ProdutoModel produto)
        {
            if (!ModelState.IsValid) return View(produto);
            _service.Editar(produto);
            TempData["MensagemSucesso"] = "Produto atualizado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            _service.Excluir(id);
            TempData["MensagemSucesso"] = "Produto desativado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
