using Microsoft.Data.Sqlite;
using ShelfLife.Controllers;
using ShelfLife.Data;
using ShelfLife.Models;

namespace ShelfLife.Tests;

/// <summary>Creates a fresh temporary database and the controllers for each test.</summary>
public class TestDb : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"shelflife_{Guid.NewGuid():N}.db");

    public ProductController Products { get; }
    public StockController Stock { get; }
    public ExpiryController Expiry { get; }

    public TestDb()
    {
        var db = new Database(_path);
        var productRepo = new ProductRepository(db);
        var saleRepo = new SaleRepository(db);
        Products = new ProductController(productRepo);
        Stock = new StockController(productRepo, saleRepo);
        Expiry = new ExpiryController(productRepo);
    }

    public int NewProduct(string name = "Milk", double price = 1000, int quantity = 10, int expiresInDays = 30) =>
        Products.AddProduct(new Product
        {
            Name = name, Category = "Dairy", Price = price, Quantity = quantity,
            ExpiryDate = DateTime.Today.AddDays(expiresInDays)
        });

    /// <summary>Makes an existing product expired (update is allowed to use past dates).</summary>
    public void MakeExpired(int id)
    {
        var p = Products.GetAllProducts().Single(x => x.Id == id);
        p.ExpiryDate = DateTime.Today.AddDays(-2);
        Products.UpdateProduct(p);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_path)) File.Delete(_path);
    }
}
