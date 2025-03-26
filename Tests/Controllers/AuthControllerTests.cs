using AccessControl.Tests.Utils;
using Domain.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Tests.Utils;
using Xunit;

namespace AccessControl.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturn200_WhenNewAdmin()
        {
            var model = new LoginModel
            {
                Username = "newadmin@access.com",
                Email = "newadmin@access.com",
                Password = "Admin123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register-admin", model);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().Contain("Admin user created successfully");
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenValidCredentials()
        {
            var model = new LoginModel
            {
                Username = "loginadmin@access.com",
                Email = "loginadmin@access.com",
                Password = "Admin123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register-admin", model);

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", model);

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
            json!.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_ShouldReturn401_WhenInvalidCredentials()
        {
            var model = new LoginModel
            {
                Username = "wronguser@access.com",
                Email = "wronguser@access.com",
                Password = "WrongPassword"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", model);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedRoute_ShouldReturn403_WhenEmailDomainIsInvalid()
        {
            var model = new LoginModel
            {
                Username = "test@gmail.com",
                Email = "test@gmail.com",
                Password = "Admin123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register-admin", model);
            var login = await _client.PostAsJsonAsync("/api/auth/login", model);
            var token = (await login.Content.ReadFromJsonAsync<TokenResponse>())!.Token;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/cards/paged");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var restricted = await _client.GetAsync("/api/doors/create?doorNumber=1&doorType=0&doorName=Test");
            restricted.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
