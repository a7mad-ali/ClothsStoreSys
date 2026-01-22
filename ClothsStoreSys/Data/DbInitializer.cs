using ClothsStoreSys.Models;
using System.Linq;

namespace ClothsStoreSys.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            var adminUser = db.Users.FirstOrDefault(u => u.Role == "Admin" || u.Username == "admin");
            if (adminUser == null)
            {
                db.Users.Add(new User { Username = "admin", PasswordHash = "admin", Role = "Admin" });
                db.SaveChanges();
            }
            else if (adminUser.PasswordHash == "disabled")
            {
                adminUser.PasswordHash = "admin";
                db.SaveChanges();
            }

            var cashierUser = db.Users.FirstOrDefault(u => u.Role == "Cashier" || u.Username == "cashier");
            if (cashierUser == null)
            {
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
