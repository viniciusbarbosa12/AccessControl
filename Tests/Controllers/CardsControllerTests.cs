using AccessControl.Tests.Utils;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AccessControl.Tests.Controllers
{
    public class CardsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CardsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task Create_ShouldReturn200_WhenValidCard()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                CardNumber = 999,
                FirstName = "Test",
                LastName = "User"
            };

            var response = await _client.PostAsJsonAsync("/api/cards/create", payload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().Contain("Card999");
        }


        [Fact]
        public async Task Create_ShouldReturn400_WhenMissingFirstName()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                CardNumber = 999,
                FirstName = "",
                LastName = "User"
            };

            var response = await _client.PostAsJsonAsync("/api/cards/create", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetPagedCards_ShouldReturn200_WithoutAuthorization()
        {
            var response = await _client.GetAsync("/api/cards/paged?page=1&pageSize=5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GrantAccess_ShouldReturn400_WhenInvalidParameters()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync("/api/cards/grant-access?cardNumber=0&doorNumber=0", null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelPermission_ShouldReturn404_WhenPermissionNotFound()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync("/api/cards/cancel-permission?cardNumber=999&doorNumber=888", null);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}