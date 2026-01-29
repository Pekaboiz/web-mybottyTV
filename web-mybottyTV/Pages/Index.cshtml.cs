using Microsoft.AspNetCore.Mvc.RazorPages;
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
        var username = "peki4";

        Channel = _settingsService.GetChannel(username);
    }
}
