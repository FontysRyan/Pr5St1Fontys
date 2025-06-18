namespace DungeonDaddies.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int AccountId { get; set; }
        public string Email {  get; set; }
    }
}