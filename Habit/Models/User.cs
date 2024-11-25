namespace Habit.Models
{
    public class User
    {
        public int Id { get; set; } // ID interno do banco
        public string AzureAdB2CId { get; set; } // 'sub' do Azure AD B2C
        public string Name { get; set; } // Nome do usuário
        public string Email { get; set; } // E-mail do usuário

        // Relacionamento com os hábitos
        public ICollection<Habits> Habits { get; set; }
    }
}
