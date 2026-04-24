// ============================================================
// HomeController.cs
// ============================================================
using LiveStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LiveStore.Models;

namespace LiveStore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboard;

        public HomeController(ILogger<HomeController> logger, IDashboardService dashboard)
        {
            _logger    = logger;
            _dashboard = dashboard;
        }

        public IActionResult Index()
        {
            var dados = _dashboard.ObterDados();
            return View(dados);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
