namespace Habit.DTOs
{
    public class CreateHabitDto
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Place { get; set; }
    }
}
