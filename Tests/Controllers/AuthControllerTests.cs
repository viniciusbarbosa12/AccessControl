using AccessControl.Tests.Utils;
using Domain.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Tests.Utils;

namespace AccessControl.Tests.Controllers
{
    [Collection("Sequential")]
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturn200_WhenNewAdmin()
        {
            _factory.ResetDatabase();

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
            _factory.ResetDatabase();

            var model = new LoginModel
            {
                Username = "newadmin",
                Email = "newadmin2@access.com",
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
            _factory.ResetDatabase();

            var model = new LoginModel
            {
                Username = "wronguser@access.com",
                Email = "wronguser@access.com",
                Password = "WrongPassword"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", model);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

    }
}

