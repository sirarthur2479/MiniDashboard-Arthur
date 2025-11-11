using System.Text.Json;
using MiniDashboard.Api.Models;

// Repository layer for Item data persistence
namespace MiniDashboard.Api.Repositories {
    public interface IItemRepository {
        IEnumerable<Item> GetAll();
        Item? GetById(int id);
        void Add(Item item);
        void Update(Item item);
        void Delete(int id);
    }

    public class ItemRepository : IItemRepository {
        private readonly string _filePath;
        private List<Item> _items;
        private int _nextId;

        public ItemRepository(IWebHostEnvironment env) {
            // Read items from a JSON file located in the data directory
            _filePath = Path.Combine(env.ContentRootPath, "data", "items.json");

            // Ensure data directory exists
            var dir = Path.GetDirectoryName(_filePath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Load or create data
            if (File.Exists(_filePath)) {
                var json = File.ReadAllText(_filePath).Trim();

                if (string.IsNullOrEmpty(json) || json == "[]" || json == "{}") {
                    _items = CreateSampleFile();
                } else {
                    _items = JsonSerializer.Deserialize<List<Item>>(json) ?? new List<Item>();

                    // If file contained invalid or empty list, recreate with sample data
                    if (_items.Count == 0)
                        _items = CreateSampleFile();
                }
            } else {
                _items = CreateSampleFile();
            }

            _nextId = _items.Any() ? _items.Max(i => i.Id) + 1 : 1;
        }

        // --- Helper methods ---
        /// <summary>
        /// Creates a sample file with default items and saves it to disk.
        /// </summary>
        private List<Item> CreateSampleFile() {
            var sampleItems = new List<Item> {
                new Item { Id = 1, Name = "Laptop", Description = "MacBook Pro 14\"", Price = 2499.99M },
                new Item { Id = 2, Name = "Headphones", Description = "Noise-cancelling Bluetooth", Price = 299.99M },
                new Item { Id = 3, Name = "Monitor", Description = "27-inch 4K Display", Price = 699.00M }
            };

            SaveChanges(sampleItems);
            return sampleItems;
        }

        /// <summary>
        /// Writes the current list of items to the JSON file.
        /// </summary>
        private void SaveChanges(List<Item> items) {
            var listToSave = items ?? _items;
            var json = JsonSerializer.Serialize(listToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<Item> GetAll() => _items;

        public Item? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);

        public void Add(Item item) {
            item.Id = _nextId++;
            _items.Add(item);
            SaveChanges(_items);
        }

        public void Update(Item item) {
            var index = _items.FindIndex(i => i.Id == item.Id);
            if (index != -1) {
                _items[index] = item;
                SaveChanges(_items);
            }
        }

        public void Delete(int id) {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null) {
                _items.Remove(item);
                SaveChanges(_items);
            }
        }
    }
}

