using System.ComponentModel.DataAnnotations;

namespace ClothsStoreSys.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        [Required]
        public string ItemCode { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int StockQty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}