using System.Diagnostics;
using Habit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous] // Permitir acesso sem autenticação
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("Usuário autenticado. Redirecionando para a tela de hábitos.");
                return RedirectToAction("Index", "Habit");
            }

            _logger.LogInformation("Visitante não autenticado acessando a página inicial.");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            _logger.LogInformation("Página Privacy acessada.");
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Página de erro acessada. Request ID: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
