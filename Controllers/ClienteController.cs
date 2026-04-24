using LiveStore.Models;
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveStore.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _service;
        public ClienteController(IClienteService service) => _service = service;

        public IActionResult Index() => View(_service.ObterTodos());

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var c = _service.ObterPorInstagram(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(ClienteModel cliente)
        {
            _service.Editar(cliente);
            TempData["MensagemSucesso"] = "Cliente atualizado!";
            return RedirectToAction(nameof(Index));
        }
    }
}
