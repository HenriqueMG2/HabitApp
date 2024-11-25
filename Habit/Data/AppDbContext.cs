using Habit.Models;
using Microsoft.EntityFrameworkCore;

namespace Habit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Habits> Habits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento entre User e Habit
            modelBuilder.Entity<User>()
                .HasMany(u => u.Habits)
                .WithOne(h => h.User)
                .HasForeignKey(h => h.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}