using BlazorBasic;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LaunchDarkly.Sdk;
using LSC = LaunchDarkly.Sdk.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddScoped<IStateService, StateService>();

var context = Context.New("context-key-123abc");
var timeSpan = TimeSpan.FromSeconds(10);
var lscConfig = LSC.Configuration
    .Builder("mob-7629f35c-edeb-41c6-9585-306b8d96d932", LSC.ConfigurationBuilder.AutoEnvAttributes.Enabled)
    .Events(LSC.Components.SendEvents().FlushInterval(TimeSpan.FromSeconds(2)))
    .Build();

LSC.LdClient client = LSC.LdClient.Init(lscConfig, context, timeSpan);

if (client.Initialized)
{
    Console.WriteLine("SDK successfully initialized!");
    builder.Services.AddSingleton(ldcClient => client);
}
else
{
    Console.WriteLine("SDK failed to initialize");
    Environment.Exit(1);
}

var flagValue = client.BoolVariation("feature-A", false);

await builder.Build().RunAsync();
