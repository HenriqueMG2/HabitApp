using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Habit.Models; // Ajuste para o namespace correto
using Habit.Data;   // Ajuste para o contexto correto
using Habit.DTOs;   // Caso use DTOs
using Microsoft.Extensions.Logging;

namespace Habit.Controllers
{
    [Authorize]
    public class HabitController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HabitController> _logger;
        private User _cachedUser;

        public HabitController(AppDbContext context, ILogger<HabitController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Método auxiliar para obter o usuário autenticado
        private User GetUser()
        {
            try
            {
                if (_cachedUser != null)
                    return _cachedUser;

                var azureAdB2CId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(azureAdB2CId))
                {
                    _logger.LogWarning("AzureAdB2CId está vazio. Redirecionando para login.");
                    return null;
                }

                _logger.LogInformation("Buscando usuário no banco com AzureAdB2CId: {AzureAdB2CId}", azureAdB2CId);
                _cachedUser = _context.Users.FirstOrDefault(u => u.AzureAdB2CId == azureAdB2CId);

                if (_cachedUser == null)
                {
                    _logger.LogWarning("Usuário não encontrado no banco.");
                }

                return _cachedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário no banco.");
                throw;
            }
        }

        // GET: /Habit
        public IActionResult Index(DateTime? date, string filter = "All")
        {
            _logger.LogInformation("[Index] Carregando hábitos do usuário.");

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Index] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            date ??= DateTime.Today;

            // Ajustando a lógica de filtragem
            var habits = _context.Habits
                .Where(h => h.UserId == user.Id)
                .Where(h => filter == "All" || (filter == "Daily" && h.StartTime.Date == date.Value.Date))
                .ToList();

            ViewBag.CurrentDate = date.Value;
            ViewBag.Filter = filter;

            _logger.LogInformation("[Index] Total de hábitos carregados: {Count}", habits.Count);

            return View(habits);
        }



        // GET: /Habit/Details/{id}
        public IActionResult Details(int id)
        {
            _logger.LogInformation("[Details] Carregando detalhes do hábito ID: {Id}", id);

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Details] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id && h.UserId == user.Id);
            if (habit == null)
            {
                _logger.LogWarning("[Details] Hábito não encontrado.");
                return NotFound();
            }

            _logger.LogInformation("[Details] Hábito encontrado: {Name}", habit.Name);
            return View(habit);
        }

        // GET: /Habit/Create
        public IActionResult Create()
        {
            _logger.LogInformation("[Create] Renderizando formulário de criação.");
            return View();
        }

        // POST: /Habit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateHabitDto dto)
        {
            _logger.LogInformation("[Create] Criando novo hábito.");

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Create] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            var habit = new Habits
            {
                Name = dto.Name,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Place = dto.Place,
                UserId = user.Id
            };

            _context.Habits.Add(habit);
            _context.SaveChanges();

            _logger.LogInformation("[Create] Hábito criado com sucesso: {Name} (ID: {Id})", habit.Name, habit.Id);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Habit/Edit/{id}
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("[Edit] Carregando hábito para edição. ID: {Id}", id);

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Edit] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id && h.UserId == user.Id);
            if (habit == null)
            {
                _logger.LogWarning("[Edit] Hábito não encontrado.");
                return NotFound();
            }

            var dto = new CreateHabitDto
            {
                Name = habit.Name,
                Place = habit.Place,
                StartTime = habit.StartTime,
                EndTime = habit.EndTime
            };

            _logger.LogInformation("[Edit] Hábito encontrado para edição: {Name}", habit.Name);
            return View(dto);
        }


        // POST: /Habit/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreateHabitDto dto)
        {
            _logger.LogInformation("[Edit] Atualizando hábito ID: {Id}", id);

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Edit] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id && h.UserId == user.Id);
            if (habit == null)
            {
                _logger.LogWarning("[Edit] Hábito não encontrado.");
                return NotFound();
            }

            habit.Name = dto.Name;
            habit.StartTime = dto.StartTime;
            habit.EndTime = dto.EndTime;
            habit.Place = dto.Place;

            _context.SaveChanges();

            _logger.LogInformation("[Edit] Hábito atualizado com sucesso.");

            return RedirectToAction(nameof(Index));
        }

        // POST: /Habit/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("[Delete] Excluindo hábito ID: {Id}", id);

            var user = GetUser();
            if (user == null)
            {
                _logger.LogWarning("[Delete] Usuário não encontrado. Redirecionando para erro.");
                return RedirectToAction("Error", "Home");
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id && h.UserId == user.Id);
            if (habit == null)
            {
                _logger.LogWarning("[Delete] Hábito não encontrado.");
                return NotFound();
            }

            _context.Habits.Remove(habit);
            _context.SaveChanges();

            _logger.LogInformation("[Delete] Hábito excluído com sucesso.");

            return RedirectToAction(nameof(Index));
        }
    }
}
