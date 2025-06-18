using DungeonDaddies.Models;
using System.Text.Json;
using System.Text;

namespace DungeonDaddies.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl = "http://192.168.133.125:5000";

        public AuthApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> AuthenticateUserAsync(LoginViewModel Inputs)
        {
            if (string.IsNullOrEmpty(Inputs.Email) || string.IsNullOrEmpty(Inputs.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email and password are required."
                };
            }
            
            var Json = JsonSerializer.Serialize(Inputs);
            var Content = new StringContent(Json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/login", Content);
            var ResponseString = await response.Content.ReadAsStringAsync();

            var LoginResult = JsonSerializer.Deserialize<LoginResponse>(ResponseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (LoginResult != null && LoginResult.Success)
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetInt32("AccountId", LoginResult.AccountId);
                    session.SetString("Email", LoginResult.Email);
                }
                
            }

            return LoginResult ?? new LoginResponse
            {
                Success = false,
                Message = LoginResult.Message ?? "An error occurred during authentication."
            };
        }

        public void Logout()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Clear();
            }
        }
    }
}
