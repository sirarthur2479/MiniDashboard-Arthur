using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Api.Models;
using MiniDashboard.Api.Services;

// Controller for handling Item-related API requests
namespace MiniDashboard.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase {
        private readonly IItemService _service;

        public ItemsController(IItemService service) {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? search = null)
            => Ok(_service.GetAll(search));

        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            var item = _service.GetById(id);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpPost]
        public IActionResult Create(Item item) {
            _service.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Item item) {
            if (id != item.Id) return BadRequest("ID mismatch");
            _service.Update(item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            _service.Delete(id);
            return NoContent();
        }
    }
}
