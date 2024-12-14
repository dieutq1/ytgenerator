using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace ytgenerator.Client.Middlewares
{
    public class AuthInterceptor : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorage;

        public AuthInterceptor(NavigationManager navigationManager, ILocalStorageService localStorage)
        {
            _navigationManager = navigationManager;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add Authorization header if token exists
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Redirect to login if response is 401
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !request.RequestUri!.AbsolutePath.Contains("login"))
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigationManager.NavigateTo("/auth/login", true);
            }

            return response;
        }
    }
}
