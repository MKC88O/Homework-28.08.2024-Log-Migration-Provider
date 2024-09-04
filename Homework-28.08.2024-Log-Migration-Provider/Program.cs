using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Homework_28._08._2024_Log_Migration_Provider
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (AppContext db = new AppContext())
            {
                Service service = new Service();
                Order order = new Order { OrderDate = DateTime.Now };
                service.AddOrder(order);

                Product product1 = new Product { Name = "Молоко", Price = 45 };
                Product product2 = new Product { Name = "Хлеб", Price = 25 };

                service.AddProductToOrder(order.Id, product1);
                service.AddProductToOrder(order.Id, product2);

                var receiveOrder = service.GetOrder(order.Id);
                if (receiveOrder != null)
                {
                    Console.WriteLine("Order ID: " + receiveOrder.Id + " Date: " + receiveOrder.OrderDate);
                }

                //service.RemoveOrder(order.Id);
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class Service
    {
        public void AddOrder(Order order)
        {
            using (AppContext db = new AppContext())
            {
                db.Orders.Add(order);
                db.SaveChanges();
            }

        }

        public void RemoveOrder(int orderId)
        {
            using (AppContext db = new AppContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    var products = db.Products.Where(p => p.OrderId == orderId).ToList();
                    db.Products.RemoveRange(products);

                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
            }
        }

        public Order? GetOrder(int orderId)
        {
            using (AppContext db = new AppContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                return order;
            }
        }

        public void AddProductToOrder(int orderId, Product product)
        {
            using (var db = new AppContext())
            {
                product.OrderId = orderId;

                db.Products.Add(product);
                db.SaveChanges();
            }
        }

    }

    public class AppContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-M496S5I;Database=Shop;Trusted_Connection=True; TrustServerCertificate=True");
        }
    }
}
