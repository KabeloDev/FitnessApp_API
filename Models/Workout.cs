namespace FitnessApp.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; } 
        //public DateTime Date { get; set; }
        public List<ExerciseEntry> Exercises { get; set; } = new();
        public string Status { get; set; } = "In Progress";
    }
}
