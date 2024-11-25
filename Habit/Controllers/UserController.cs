using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Habit.Data; // Ajuste para o seu namespace
using Habit.Models;

namespace Habit.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para verificar ou criar o usuário no banco
        public IActionResult Profile()
        {
            try
            {
                _logger.LogInformation("Iniciando método Profile.");

                var azureAdB2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var name = User.FindFirst(ClaimTypes.GivenName)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                _logger.LogInformation("Dados do usuário obtidos: AzureAdB2CId={AzureAdB2CId}, Name={Name}, Email={Email}",
                    azureAdB2CId, name, email);

                if (string.IsNullOrEmpty(azureAdB2CId))
                {
                    _logger.LogWarning("AzureAdB2CId está vazio ou nulo.");
                    return RedirectToAction("Error", "Home");
                }

                // Verifica se o usuário já existe no banco
                var user = _context.Users.FirstOrDefault(u => u.AzureAdB2CId == azureAdB2CId);

                if (user == null)
                {
                    _logger.LogInformation("Usuário não encontrado no banco. Criando novo usuário.");
                    user = new User
                    {
                        AzureAdB2CId = azureAdB2CId,
                        Name = name,
                        Email = email
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();
                    _logger.LogInformation("Novo usuário criado e salvo no banco: {User}", user);
                }
                else
                {
                    _logger.LogInformation("Usuário encontrado no banco: {UserName}", user.Name);
                }

                // Retorna para uma view do perfil do usuário
                _logger.LogInformation("Retornando a view do perfil do usuário.");
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no método Profile.");
                throw;
            }
        }
    }
}
