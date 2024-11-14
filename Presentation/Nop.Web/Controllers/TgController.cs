using Microsoft.AspNetCore.Mvc;
using Nop.Web.Service;

namespace Nop.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TgController : ControllerBase
{
    private readonly WTelegramService WT;
    private readonly HttpClient _httpClient;

    public TgController(WTelegramService wt, HttpClient httpClient)
    {
        WT = wt;
        _httpClient = httpClient;
    }

    [HttpGet("status")]
    public ContentResult Status()
    {
        switch (WT.ConfigNeeded)
        {
            case "connecting": return Content("<meta http-equiv=\"refresh\" content=\"1\">WTelegram is connecting...", "text/html");
            case null: return Content($@"Connected as {WT.User}<br/><a href=""chats"">Get all chats</a>", "text/html");
            default: return Content($@"Enter {WT.ConfigNeeded}: <form action=""config""><input name=""value"" autofocus/></form>", "text/html");
        }
    }

    [HttpGet("config")]
    public async Task<ActionResult> Config(string value)
    {
        var line = await WT.DoLogin(value);
        return Redirect("status");
    }

    [HttpGet("chats")]
    public async Task<object> Chats()
    {
        if (WT.User == null) throw new Exception("Complete the login first");
        var chats = await WT.Client.Messages_GetAllChats();
        return chats.chats;
    }
}