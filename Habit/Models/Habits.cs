namespace Habit.Models
{
    public class Habits
    {
        public int Id { get; set; } // ID do hábito
        public string Name { get; set; } // Nome do hábito
        public DateTime StartTime { get; set; } // Horário inicial
        public DateTime EndTime { get; set; } // Horário final
        public string Place { get; set; } // Local do hábito

        // Relacionamento com o usuário
        public int UserId { get; set; } // FK para o usuário no banco
        public User User { get; set; }
    }
}
