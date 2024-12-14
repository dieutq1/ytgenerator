using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestEase;
using System.Text;
using ytgenerator.Client.DataService;
using ytgenerator.Client.Middlewares;
using ytgenerator.Components;
using ytgenerator.Controllers;
using ytgenerator.Data;

namespace ytgenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Authentication and JWT Bearer
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? string.Empty,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"] ?? string.Empty,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Add Authorization
            builder.Services.AddAuthorizationCore();

            // Register AuthenticationStateProvider
            builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            // Add Controllers and API Explorer
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger with Authentication
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "YTGenerator API", Version = "2024" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and your JWT token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add services
            builder.Services.AddScoped<Services.IUserServices, Services.UserServices>();
            builder.Services.AddScoped<Services.IAccessTokenService, Services.AccessTokenService>();

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

                var baseAddress = builder.Configuration["BaseUrl"];

                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(baseAddress)
                };

                return RestClient.For<IDataServices>(httpClient);
            });

            builder.Services.AddHttpClient("ExternalApi", client =>
            {
                var pyApiUrl = builder.Configuration["PyApiUrl"];
                client.BaseAddress = new Uri(pyApiUrl);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapControllers();

            app.Run();
        }
    }
}