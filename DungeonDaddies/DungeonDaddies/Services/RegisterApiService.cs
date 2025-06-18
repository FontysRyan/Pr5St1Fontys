using DungeonDaddies.Models;
using System.Text.Json;
using System.Text;

namespace DungeonDaddies.Services
{
    public class RegisterApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl = "http://192.168.133.125:5000";

        public RegisterApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RegisterResponse> RegisterUserAsync(RegisterViewModel Inputs)
        {
            // Check if all values are set in Inputs
            if (string.IsNullOrEmpty(Inputs.Email)
                || string.IsNullOrEmpty(Inputs.Password)
                || string.IsNullOrEmpty(Inputs.ConfirmPassword)
                || string.IsNullOrEmpty(Inputs.Username)
                || string.IsNullOrEmpty(Inputs.FirstName)
                || string.IsNullOrEmpty(Inputs.LastName))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "All fields are required."
                };
            }

            var Json = JsonSerializer.Serialize(Inputs);
            var Content = new StringContent(Json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/register", Content);
            var ResponseString = await response.Content.ReadAsStringAsync();

            var RegisterResult = JsonSerializer.Deserialize<RegisterResponse>(ResponseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (RegisterResult != null && RegisterResult.Success)
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetInt32("AccountId", RegisterResult.AccountId);
                    session.SetString("Email", RegisterResult.Email);
                }

            }

            return RegisterResult ?? new RegisterResponse
            {
                Success = false,
                Message = RegisterResult.Message ?? "An error occurred during registering."
            };
        }
    }
}
