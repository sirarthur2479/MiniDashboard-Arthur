using System.Net;
using System.Net.Http.Json;
using Xunit;
using MiniDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

// Integration tests for Item API endpoints
namespace MiniDashboard.IntegrationTests {
    public class ItemApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private readonly HttpClient _client;

        public ItemApiIntegrationTests(WebApplicationFactory<Program> factory) {
            _client = factory.CreateClient();
        }

        [Fact]
        // Test basic request and response for GetAll endpoint
        public async Task GetAll_ReturnsOk() {
            var response = await _client.GetAsync("/api/items");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        // Full CRUD flow test
        // End to-end test for creating, reading, updating, and deleting an item
        public async Task CRUD_FullFlow_Works() {
            // --- CREATE ---
            var newItem = new Item { Name = "TestItem", Description = "Integration Test", Price = 99.9m };
            var postResponse = await _client.PostAsJsonAsync("/api/items", newItem);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var createdItem = await postResponse.Content.ReadFromJsonAsync<Item>();
            Assert.NotNull(createdItem);
            Assert.Equal("TestItem", createdItem!.Name);

            // --- GET BY ID ---
            var getResponse = await _client.GetAsync($"/api/items/{createdItem.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getItem = await getResponse.Content.ReadFromJsonAsync<Item>();
            Assert.NotNull(getItem);
            Assert.Equal("TestItem", getItem!.Name);

            // --- UPDATE ---
            createdItem.Name = "UpdatedItem";
            var putResponse = await _client.PutAsJsonAsync($"/api/items/{createdItem.Id}", createdItem);
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            var getUpdated = await _client.GetAsync($"/api/items/{createdItem.Id}");
            var updatedItem = await getUpdated.Content.ReadFromJsonAsync<Item>();
            Assert.Equal("UpdatedItem", updatedItem!.Name);

            // --- DELETE ---
            var deleteResponse = await _client.DeleteAsync($"/api/items/{createdItem.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getDeleted = await _client.GetAsync($"/api/items/{createdItem.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getDeleted.StatusCode);
        }
    }
}
