using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using MiniDashboard.App.Models;

// Service layer for interacting with the MiniDashboard API
namespace MiniDashboard.App.Services {
    public class ApiService {
        private readonly HttpClient _httpClient;

        // local cache file path
        private readonly string _cacheFilePath;

        public ApiService() {
            _httpClient = new HttpClient {
                BaseAddress = new Uri("https://localhost:7264/")
            };
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MiniDashboardApp",
                "Cache"
            );

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            _cacheFilePath = Path.Combine(appDataFolder, "item_cache.json");
        }

        public async Task<List<ItemModel>> GetItemsAsync(string? search = null) {
            try {
                string url = "api/items";
                if (!string.IsNullOrWhiteSpace(search)) {
                    url += $"?search={search}";
                }

                var items = await _httpClient.GetFromJsonAsync<List<ItemModel>>(url) ?? new List<ItemModel>();
                File.WriteAllText(_cacheFilePath, JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true }));

                if (!string.IsNullOrWhiteSpace(search))
                    items = items.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                return items;
            } catch {
                // On failure, load from local cache
                if (File.Exists(_cacheFilePath)) {
                    var cached = JsonSerializer.Deserialize<List<ItemModel>>(File.ReadAllText(_cacheFilePath)) ?? new();
                    if (!string.IsNullOrWhiteSpace(search))
                        cached = cached.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                    return cached;
                }
                throw;
            }
        }

        public async Task<ItemModel?> AddItemAsync(ItemModel item) {
            var response = await _httpClient.PostAsJsonAsync("api/items", item);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ItemModel>();
        }

        public async Task UpdateItemAsync(ItemModel item) {
            var response = await _httpClient.PutAsJsonAsync($"api/items/{item.Id}", item);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteItemAsync(int id) {
            var response = await _httpClient.DeleteAsync($"api/items/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
