using AccessControl.Tests.Utils;
using Domain.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AccessControl.Tests.Controllers
{
    public class DoorsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DoorsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateDoor_ShouldReturn200_WhenValid()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync("/api/doors/create?doorNumber=1&doorType=0&doorName=MainEntrance", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateDoor_ShouldReturn400_WhenInvalidInput()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync("/api/doors/create?doorNumber=0&doorType=-1&doorName=", null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteDoor_ShouldReturn404_WhenDoorDoesNotExist()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync("/api/doors/remove?doorNumber=999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SimulateSwipe_ShouldReturn400_WhenInvalidInput()
        {
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsJsonAsync("/api/doors/swipe?doorNumber=0", new Card());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}