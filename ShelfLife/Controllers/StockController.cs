using ShelfLife.Data;
using ShelfLife.Models;

namespace ShelfLife.Controllers;

/// <summary>Feature: Stock and Sales (REQ-STK-1 to REQ-STK-3)</summary>
public class StockController
{
    private readonly ProductRepository _products;
    private readonly SaleRepository _sales;

    public StockController(ProductRepository products, SaleRepository sales)
    {
        _products = products;
        _sales = sales;
    }

    /// <summary>REQ-STK-1: Add stock to a product (restock).</summary>
    public void Restock(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0.");
        var product = _products.GetById(productId)
            ?? throw new InvalidOperationException("Product not found.");
        if (product.ExpiryDate.Date < DateTime.Today)
            throw new InvalidOperationException("This product is expired. Update its expiry date before restocking.");
        _products.SetQuantity(productId, product.Quantity + quantity);
    }

    /// <summary>
    /// REQ-STK-2: Sell a product. Expired products cannot be sold and the stock cannot go below 0.
    /// Returns the total price.
    /// </summary>
    public double SellProduct(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0.");
        var product = _products.GetById(productId)
            ?? throw new InvalidOperationException("Product not found.");
        if (product.ExpiryDate.Date < DateTime.Today)
            throw new InvalidOperationException("This product is expired and cannot be sold.");
        if (quantity > product.Quantity)
            throw new InvalidOperationException($"Not enough stock. Only {product.Quantity} left.");

        double total = product.Price * quantity;
        _sales.Record(productId, quantity, total, DateTime.Now);
        return total;
    }

    /// <summary>REQ-STK-3: View the sales history (newest first).</summary>
    public List<Sale> GetSales() => _sales.GetAll();

    public List<Product> GetProducts() => _products.GetAll();
}
