using ClothsStoreSys.Models;
using System.Linq;

namespace ClothsStoreSys.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Username = "admin", PasswordHash = "admin", Role = "Admin" });
                db.Users.Add(new User { Username = "cashier", PasswordHash = "cashier", Role = "Cashier" });
                db.SaveChanges();
            }

            if (!db.Items.Any())
            {
                db.Items.Add(new Item { Name = "T-Shirt", Category = "Top", Size = "M", Color = "Blue", ItemCode = "TSHIRT-001", StockQty = 50, UnitPrice = 10 });
                db.Items.Add(new Item { Name = "Jeans", Category = "Bottom", Size = "L", Color = "Black", ItemCode = "JEANS-001", StockQty = 30, UnitPrice = 25 });
                db.SaveChanges();
            }
        }
    }
}