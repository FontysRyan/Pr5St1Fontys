using DungeonDaddies.Models;
using DungeonDaddies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DungeonDaddies.Pages.Register
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        private readonly RegisterApiService _registerApiService;

        public IndexModel(RegisterApiService RegisterApiService)
        {
            _registerApiService = RegisterApiService;
        }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var RegisterResponse = await _registerApiService.RegisterUserAsync(Input);

            if (RegisterResponse.Success)
            {
                return RedirectToPage("/Runs/Index");
            }

            ErrorMessage = RegisterResponse.Message;
            return Page();
        }
    }
}
