using ShelfLife.Controllers;
using ShelfLife.Data;
using ShelfLife.Views;

namespace ShelfLife;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Model/Data layer
        var database = new Database();
        var productRepo = new ProductRepository(database);
        var saleRepo = new SaleRepository(database);

        // Controllers
        var productController = new ProductController(productRepo);
        var stockController = new StockController(productRepo, saleRepo);
        var expiryController = new ExpiryController(productRepo);

        // View
        Application.Run(new MainForm(productController, stockController, expiryController));
    }
}
