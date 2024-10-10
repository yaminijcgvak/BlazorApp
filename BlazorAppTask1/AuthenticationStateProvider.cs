using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
#nullable disable
namespace BlazorAppTask1
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Retrieve the token from session storage
            var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            if (!string.IsNullOrEmpty(token))
            {
                // Validate and parse the token to extract claims
                var payload = GetTokenPayload(token);
                if (payload != null)
                {
                    user = new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromPayload(payload), "jwt"));
                }
            }

            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromPayload(GetTokenPayload(token)), "jwt"));
            var state = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(state);
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var state = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(state);
        }

        private Dictionary<string, object> GetTokenPayload(string token)
        {
            var payload = token.Split('.')[1]; 
            var jsonBytes = ParseBase64WithoutPadding(payload);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes); 
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> GetClaimsFromPayload(Dictionary<string, object> payload)
        {
            var claims = new List<Claim>();
            if (payload.ContainsKey("name"))
            {
                claims.Add(new Claim(ClaimTypes.Name, payload["name"].ToString()));
            }
            if (payload.ContainsKey("role"))
            {
                claims.Add(new Claim(ClaimTypes.Role, payload["role"].ToString()));
            }

            return claims;
        }
    }
}
