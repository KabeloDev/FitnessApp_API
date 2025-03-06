namespace FitnessApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfileImageUrl { get; set; } = "/Images/default.jpg"; 
    }
}
