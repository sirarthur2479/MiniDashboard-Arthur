using Xunit;
using Moq;
using MiniDashboard.Api.Models;
using MiniDashboard.Api.Repositories;
using MiniDashboard.Api.Services;

// Unit tests for ItemService
namespace MiniDashboard.Tests {
    public class ItemServiceTests {
        private readonly ItemService _service;
        private readonly Mock<IItemRepository> _repoMock;

        public ItemServiceTests() {
            _repoMock = new Mock<IItemRepository>();
            _service = new ItemService(_repoMock.Object);
        }

        [Fact]
        public void GetAll_ReturnsAllItems() {
            // Arrange
            var items = new List<Item> { new() { Id = 1, Name = "Laptop" } };
            _repoMock.Setup(r => r.GetAll()).Returns(items);

            // Act
            var result = _service.GetAll();

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop", result.First().Name);
        }

        [Fact]
        public void GetAll_WithSearch_FiltersItems() {
            // Arrange
            var items = new List<Item> {
                new() { Id = 1, Name = "Laptop" },
                new() { Id = 2, Name = "Phone" }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(items);

            // Act
            var result = _service.GetAll("Laptop");

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop", result.First().Name);
        }

        [Fact]
        public void GetById_ItemExists_ReturnsItem() {
            // Arrange
            var item = new Item { Id = 1, Name = "Laptop" };
            _repoMock.Setup(r => r.GetById(1)).Returns(item);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop", result!.Name);
        }

        [Fact]
        public void GetById_ItemDoesNotExist_ReturnsNull() {
            // Arrange
            _repoMock.Setup(r => r.GetById(1)).Returns((Item?)null);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Add_CallsRepositoryAdd() {
            // Arrange
            var newItem = new Item { Name = "Tablet" };

            // Act
            _service.Add(newItem);

            // Assert
            _repoMock.Verify(r => r.Add(newItem), Times.Once);
        }

        [Fact]
        public void Update_CallsRepositoryUpdate() {
            // Arrange
            var item = new Item { Id = 1, Name = "Laptop" };

            // Act
            _service.Update(item);

            // Assert
            _repoMock.Verify(r => r.Update(item), Times.Once);
        }

        [Fact]
        public void Delete_CallsRepositoryDelete() {
            // Arrange

            // Act
            _service.Delete(1);

            // Assert
            _repoMock.Verify(r => r.Delete(1), Times.Once);
        }

        [Fact]
        public void GetAll_WithEmptySearch_ReturnsAllItems() {
            // Arrange
            var items = new List<Item> {
                new Item { Id = 1, Name = "Laptop" },
                new Item { Id = 2, Name = "Phone" }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(items);

            // Act
            var result = _service.GetAll(""); // empty string

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetAll_WithNullSearch_ReturnsAllItems() {
            // Arrange
            var items = new List<Item> {
                new Item { Id = 1, Name = "Laptop" },
                new Item { Id = 2, Name = "Phone" }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(items);

            // Act
            var result = _service.GetAll(null); // null search

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}