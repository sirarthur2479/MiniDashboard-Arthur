// Definition of the Item model used in the MiniDashboard APP.
namespace MiniDashboard.App.Models {
    public class ItemModel {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
