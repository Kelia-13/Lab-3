using ShelfLife.Data;
using ShelfLife.Models;

namespace ShelfLife.Controllers;

/// <summary>Feature: Product Management (REQ-PRD-1 to REQ-PRD-5)</summary>
public class ProductController
{
    private readonly ProductRepository _repo;

    public ProductController(ProductRepository repo) => _repo = repo;

    /// <summary>REQ-PRD-1: Add a new product to stock.</summary>
    public int AddProduct(Product product)
    {
        Validate(product);
        if (product.ExpiryDate.Date < DateTime.Today)
            throw new ArgumentException("Expiry date must be today or later.");
        return _repo.Add(product);
    }

    /// <summary>REQ-PRD-2: Update an existing product.</summary>
    public void UpdateProduct(Product product)
    {
        Validate(product);
        if (_repo.GetById(product.Id) == null)
            throw new InvalidOperationException("Product not found.");
        _repo.Update(product);
    }

    /// <summary>REQ-PRD-3: Delete a product that has no sales history.</summary>
    public void DeleteProduct(int id)
    {
        if (_repo.GetById(id) == null)
            throw new InvalidOperationException("Product not found.");
        if (_repo.HasSales(id))
            throw new InvalidOperationException("This product has sales history and cannot be deleted. Set its quantity to 0 instead.");
        _repo.Delete(id);
    }

    /// <summary>REQ-PRD-4: View all products.</summary>
    public List<Product> GetAllProducts() => _repo.GetAll();

    /// <summary>REQ-PRD-5: Search products by name or category.</summary>
    public List<Product> SearchProducts(string text) =>
        string.IsNullOrWhiteSpace(text) ? _repo.GetAll() : _repo.Search(text.Trim());

    private static void Validate(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) throw new ArgumentException("Product name is required.");
        if (string.IsNullOrWhiteSpace(p.Category)) throw new ArgumentException("Category is required.");
        if (p.Price <= 0) throw new ArgumentException("Price must be greater than 0.");
        if (p.Quantity < 0) throw new ArgumentException("Quantity cannot be negative.");
        p.Name = p.Name.Trim();
        p.Category = p.Category.Trim();
    }
}
