using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        [Required]
        public string ItemCode { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int StockQty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
