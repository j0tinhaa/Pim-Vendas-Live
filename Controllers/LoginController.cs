using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LiveStore.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // ── Credenciais fixas (mover para appsettings em produção) ────────────
        private const string AdminUser = "admin";
        private const string AdminPass = "admin";

        // GET /Login
        [HttpGet]
        public IActionResult Index(string? returnUrl)
        {
            // Se já autenticado, vai direto para home
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string usuario, string senha, string? returnUrl)
        {
            if (usuario == AdminUser && senha == AdminPass)
            {
                // Cria os claims do usuário autenticado
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,           usuario),
                    new Claim(ClaimTypes.Role,           "Admin"),
                    new Claim("FullName",                "Administrador")
                };

                var identity  = new ClaimsIdentity(claims, "LiveStoreCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("LiveStoreCookie", principal,
                    new AuthenticationProperties
                    {
                        IsPersistent  = true,
                        ExpiresUtc    = DateTimeOffset.UtcNow.AddHours(8)
                    });

                // Redireciona para a URL original ou para home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro      = "Usuário ou senha incorretos.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Login/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("LiveStoreCookie");
            return RedirectToAction(nameof(Index));
        }
    }
}
