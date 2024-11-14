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
    
    private string Phone { get; set; }

    public WTelegramService(IConfiguration config, ILogger<WTelegramService> logger)
    {
        _config = config;
        this.scopeFactory = scopeFactory;
        WTelegram.Helpers.Log = (lvl, msg) => logger.Log((LogLevel)lvl, msg);
        Client = new WTelegram.Client(what => Config(what));
    }

    public override void Dispose()
    {
        Client.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConfigNeeded = await DoLogin(Config("phone_number"));
    }
    
    private string Config(string what)
    {
        switch (what)
        {
            case "api_id": return "25421922";
            case "api_hash": return "8ed9b2cb68b4b22166105e81ddafb969";
            case "phone_number": return "+79161176266";
            default: return null;                  // let WTelegramClient decide the default config
        }
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