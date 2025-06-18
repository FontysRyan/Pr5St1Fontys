namespace DungeonDaddies.Models
{
    public class User
    {
        public int Id { get; set; }
        required public string Username { get; set; }
        required public string Email { get; set; }
        required public string Password_Hash { get; set; }
        required public string First_Name {  get; set; }
        required public string Last_Name { get; set; }
        required public int Role { get; set; }
    }
}
