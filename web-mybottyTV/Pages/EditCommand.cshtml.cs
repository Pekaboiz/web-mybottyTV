using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web_mybottyTV.Pages
{
    public class EditCommandModel : PageModel
    {
        public string? CommandName { get; private set; }

        public void OnGet(string command)
        {
            CommandName = command;
        }
    }
}
