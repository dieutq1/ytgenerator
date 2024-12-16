using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestEase;
using ytgenerator.Client.DataService;
using ytgenerator.Client.Middlewares;

namespace ytgenerator.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<AuthInterceptor>();

            builder.Services.AddScoped(sp =>
            {
                var localStorage = sp.GetRequiredService<ILocalStorageService>();
                var navSv = sp.GetRequiredService<NavigationManager>();
                var handler = new AuthInterceptor(navSv, localStorage)
                {
                    InnerHandler = new HttpClientHandler()
                };

                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                };

                return RestClient.For<IDataServices>(httpClient);
            });

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            await builder.Build().RunAsync();
        }
    }

}