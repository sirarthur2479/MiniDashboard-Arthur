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
        private readonly List<Item> _items;
        private int _nextId;

        public ItemRepository(IWebHostEnvironment env) {
            // Read items from a JSON file located in the data directory
            _filePath = Path.Combine(env.ContentRootPath, "data", "items.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            if (File.Exists(_filePath)) {
                var json = File.ReadAllText(_filePath);
                _items = JsonSerializer.Deserialize<List<Item>>(json) ?? new List<Item>();
            } else {
                //create empty file if it doesn't exist
                _items = new List<Item>();
                SaveChanges();
            }

            _nextId = _items.Any() ? _items.Max(i => i.Id) + 1 : 1;
        }

        // Save changes to the JSON file to persist data
        private void SaveChanges() {
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<Item> GetAll() => _items;

        public Item? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);

        public void Add(Item item) {
            item.Id = _nextId++;
            _items.Add(item);
            SaveChanges();
        }

        public void Update(Item item) {
            var index = _items.FindIndex(i => i.Id == item.Id);
            if (index != -1) {
                _items[index] = item;
                SaveChanges();
            }
        }

        public void Delete(int id) {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null) {
                _items.Remove(item);
                SaveChanges();
            }
        }
    }
}
