using System.Net.Http.Json;

namespace AccessControl.Tests.Utils
{
    public static class JwtHelper
    {
        public static async Task<string> GetAdminTokenAsync(HttpClient client)
        {
            var credentials = new
            {
                username = "admin@access.com",
                email = "admin@access.com",
                password = "Admin123!"
            };

            await client.PostAsJsonAsync("/api/auth/register-admin", credentials);

            var response = await client.PostAsJsonAsync("/api/auth/login", credentials);

            var data = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return data?.Token ?? throw new Exception("Failed to get token");
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
