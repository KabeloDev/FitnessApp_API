namespace FitnessApp.Models
{
    public class Planner
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
