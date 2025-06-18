using DungeonDaddies.Models;
using DungeonDaddies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;

namespace DungeonDaddies.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly AuthApiService _authApiService;

        public IndexModel(AuthApiService AuthApiService)
        {
            _authApiService = AuthApiService;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var loginResponse = await _authApiService.AuthenticateUserAsync(Input);

            if (loginResponse.Success)
            {
                return RedirectToPage("/Runs/Index");
            }

            ErrorMessage = loginResponse.Message;
            return Page();
        }
    }
}
