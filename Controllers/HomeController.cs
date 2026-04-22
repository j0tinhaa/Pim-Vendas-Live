using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PimVendas.Models;
using PimVendas.Services;

namespace PimVendas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVendaService _vendaService;

        public HomeController(ILogger<HomeController> logger, IVendaService vendaService)
        {
            _logger = logger;
            _vendaService = vendaService;
        }

        public IActionResult Index()
        {
            var dashboard = _vendaService.ObterDadosDashboard();
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}