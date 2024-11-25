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

        [AllowAnonymous] // Permitir acesso sem autentica��o
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("Usu�rio autenticado. Redirecionando para a tela de h�bitos.");
                return RedirectToAction("Index", "Habit");
            }

            _logger.LogInformation("Visitante n�o autenticado acessando a p�gina inicial.");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            _logger.LogInformation("P�gina Privacy acessada.");
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("P�gina de erro acessada. Request ID: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
