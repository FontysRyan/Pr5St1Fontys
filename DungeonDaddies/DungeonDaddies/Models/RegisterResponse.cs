namespace DungeonDaddies.Models
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
