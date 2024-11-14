using Autofac.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Service;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WTelegramService>();
builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
{
    var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
    builder.Configuration.AddJsonFile(path, true, true);
}
builder.Configuration.AddEnvironmentVariables();
        
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

//load application settings
builder.Services.ConfigureApplicationSettings(builder);

var appSettings = Singleton<AppSettings>.Instance;
var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;

if (useAutofac)
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
else
    builder.Host.UseDefaultServiceProvider(options =>
    {
        //we don't validate the scopes, since at the app start and the initial configuration we need 
        //to resolve some services (registered as "scoped") through the root container
        options.ValidateScopes = false;
        options.ValidateOnBuild = true;
    });

//add services to the application and configure service provider
builder.Services.ConfigureApplicationServices(builder);

var app = builder.Build();

//configure the application HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(builder => builder
    .AllowAnyOrigin() // Разрешить любой исходный домен
    .AllowAnyMethod() // Разрешить любой метод
    .AllowAnyHeader());
        
app.ConfigureRequestPipeline();
await app.StartEngineAsync();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();