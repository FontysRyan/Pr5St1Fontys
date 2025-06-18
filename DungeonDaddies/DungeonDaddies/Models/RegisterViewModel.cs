using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DungeonDaddies.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [JsonIgnore] // Don’t send this to the API
        public string ConfirmPassword { get; set; }

        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [Required]
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [Required]
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}
