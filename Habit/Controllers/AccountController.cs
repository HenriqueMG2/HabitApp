using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Habit.Data; // Ajuste para o namespace correto
using Habit.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Habit.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login()
        {
            _logger.LogInformation("Iniciando processo de login.");
            var redirectUrl = Url.Action("PostLogin", "Account");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Register()
        {
            _logger.LogInformation("Iniciando processo de registro.");
            var redirectUrl = Url.Action("PostLogin", "Account");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            _logger.LogInformation("Usuário efetuando logout.");
            return SignOut(new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") }, OpenIdConnectDefaults.AuthenticationScheme, "Cookies");
        }

        public IActionResult ResetPassword()
        {
            _logger.LogInformation("Iniciando processo de redefinição de senha.");
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Pós-login: verificar/criar o usuário e redirecionar para o dashboard
        public IActionResult PostLogin()
        {
            try
            {
                //foreach (var claim in User.Claims)
                //{
                //    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                //}

                var azureAdB2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var name = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown User";
                var email = User.FindFirst("emails")?.Value;

                if (string.IsNullOrEmpty(azureAdB2CId))
                {
                    _logger.LogError("AzureAdB2CId não encontrado no token.");
                    return RedirectToAction("Error", "Home", new { message = "ID de usuário inválido." });
                }

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Email obrigatório não encontrado no token.");
                    return RedirectToAction("Error", "Home", new { message = "Email é obrigatório para continuar." });
                }

                _logger.LogInformation($"PostLogin iniciado. AzureAdB2CId: {azureAdB2CId}");

                var user = _context.Users.FirstOrDefault(u => u.AzureAdB2CId == azureAdB2CId);

                if (user == null)
                {
                    _logger.LogInformation("Usuário não encontrado. Criando novo usuário.");
                    user = new User
                    {
                        AzureAdB2CId = azureAdB2CId,
                        Name = name,
                        Email = email
                    };

                    _context.Users.Add(user);
                }
                else
                {
                    _logger.LogInformation($"Usuário encontrado: {user.Name}. Atualizando informações.");
                    user.Name = name;
                    user.Email = email;
                    _context.Users.Update(user);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar/atualizar usuário no PostLogin: {ex.Message}");
                return RedirectToAction("Error", "Home", new { message = "Erro ao processar sua solicitação." });
            }

            return RedirectToAction("Index", "Habit");
        }


    }
}
