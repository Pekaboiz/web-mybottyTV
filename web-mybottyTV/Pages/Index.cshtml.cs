using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Utils;
using web_mybottyTV.Utils;

namespace web_mybottyTV.Pages;

public class IndexModel : PageModel
{
    private readonly BotSettingsService _settingsService;

    public BotSettings? Channel { get; private set; }

    public IndexModel(BotSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void OnGet()
    {
        var login = User.Identity?.Name;

        if (login is not null)
        {
            Channel = _settingsService.GetChannel(login);
        }
    }
}
