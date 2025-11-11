using MiniDashboard.Api.Models;
using MiniDashboard.Api.Repositories;

// Service layer for Item operations
namespace MiniDashboard.Api.Services {
    public interface IItemService {
        IEnumerable<Item> GetAll(string? search = null);
        Item? GetById(int id);
        void Add(Item item);
        void Update(Item item);
        void Delete(int id);
    }

    public class ItemService : IItemService {
        private readonly IItemRepository _repository;

        public ItemService(IItemRepository repository) {
            _repository = repository;
        }

        public IEnumerable<Item> GetAll(string? search = null) {
            var items = _repository.GetAll();
            if (!string.IsNullOrEmpty(search))
                items = items.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            return items;
        }

        public Item? GetById(int id) => _repository.GetById(id);
        public void Add(Item item) => _repository.Add(item);
        public void Update(Item item) => _repository.Update(item);
        public void Delete(int id) => _repository.Delete(id);
    }
}
