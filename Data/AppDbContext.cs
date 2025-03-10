using FitnessApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<ExerciseEntry> ExerciseEntries { get; set; }
        public DbSet<Planner> Planners { get; set; }
    }
}
