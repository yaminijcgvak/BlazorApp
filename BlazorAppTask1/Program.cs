using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAppTask1;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorAppTask1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7231/api/") });

            builder.Services.AddAuthorizationCore(); 
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>(); 

            await builder.Build().RunAsync();
        }
    }
}
