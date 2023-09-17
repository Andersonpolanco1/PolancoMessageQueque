namespace Consumer1App.Models
{
    public enum Action { Deleted, Created }

    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Action UserAction { get; set; }
        public DateTime Date { get; set; }
    }
}
