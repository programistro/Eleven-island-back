using Nop.Web.Models;
using TL;
using WTelegram.Types;

namespace Nop.Web.Service;

public class WTelegramService : BackgroundService
{
    public readonly WTelegram.Client Client;
    public TL.User User => Client.User;
    static WTelegram.UpdateManager Manager;
    private readonly IServiceScopeFactory scopeFactory;

    public string ConfigNeeded = "connecting";
    private readonly IConfiguration _config;

    public WTelegramService(IConfiguration config, ILogger<WTelegramService> logger)
    {
        _config = config;
        this.scopeFactory = scopeFactory;
        WTelegram.Helpers.Log = (lvl, msg) => logger.Log((LogLevel)lvl, msg);
        Client = new WTelegram.Client(what => _config[what]);
    }

    public override void Dispose()
    {
        Client.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConfigNeeded = await DoLogin(_config["phone_number"]);
    }
    
    public async Task<string> DoLogin(string loginInfo)
    {
        return ConfigNeeded = await Client.Login(loginInfo);
    }

    public async Task SendMessage(OrderTg orderTg)
    {
        var chats = await Client.Messages_GetAllChats(); // chats = groups/channels (does not include users dialogs)
        Console.WriteLine("This user has joined the following:");
        var chat = chats.chats[1].ToInputPeer();

        await Client.SendMessageAsync(chat, "");
    }
}