using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Api.Controllers;
using MiniDashboard.Api.Models;
using MiniDashboard.Api.Services;

// Unit tests for ItemsController
namespace MiniDashboard.Tests {
    public class ItemsControllerTests {
        private readonly ItemsController _controller;
        private readonly Mock<IItemService> _serviceMock;

        public ItemsControllerTests() {
            _serviceMock = new Mock<IItemService>();
            _controller = new ItemsController(_serviceMock.Object);
        }

        [Fact]
        public void GetAll_ReturnsOkResult() {
            // Arrange
            _serviceMock.Setup(s => s.GetAll(null)).Returns(new List<Item>());

            // Act
            var result = _controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetById_ItemExists_ReturnsOk() {
            // Arrange
            var item = new Item { Id = 1, Name = "Laptop" };
            _serviceMock.Setup(s => s.GetById(1)).Returns(item);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(item, okResult.Value);
        }

        [Fact]
        public void GetById_ItemDoesNotExist_ReturnsNotFound() {
            // Arrange
            _serviceMock.Setup(s => s.GetById(1)).Returns((Item?)null);

            // Act
            var result = _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ReturnsCreatedAtAction() {
            // Arrange
            var item = new Item { Name = "Tablet" };

            // Act
            var result = _controller.Create(item);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(item, createdResult.Value);
        }

        [Fact]
        public void Update_ValidId_CallsServiceUpdate_ReturnsNoContent() {
            // Arrange
            var item = new Item { Id = 1, Name = "Laptop" };

            // Act
            var result = _controller.Update(1, item);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.Update(item), Times.Once);
        }

        [Fact]
        public void Update_IdMismatch_ReturnsBadRequest() {
            // Arrange
            var item = new Item { Id = 2, Name = "Laptop" };

            // Act
            var result = _controller.Update(1, item);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_CallsServiceDelete_ReturnsNoContent() {
            // Arrange

            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.Delete(1), Times.Once);
        }

        [Fact]
        public void GetAll_WithSearch_FiltersItems() {
            // Arrange
            var items = new List<Item> {
                new Item { Id = 1, Name = "Laptop" },
                new Item { Id = 2, Name = "Phone" }
            };
            _serviceMock.Setup(s => s.GetAll("Laptop")).Returns(items.Where(i => i.Name == "Laptop"));

            // Act
            var result = _controller.GetAll("Laptop");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert.IsAssignableFrom<IEnumerable<Item>>(okResult.Value);
            Assert.Single(returnedItems);
        }
    }
}