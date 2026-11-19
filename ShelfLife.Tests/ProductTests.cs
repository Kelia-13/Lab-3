using ShelfLife.Models;

namespace ShelfLife.Tests;

public class ProductTests : IDisposable
{
    private readonly TestDb _t = new();
    public void Dispose() => _t.Dispose();

    [Fact]
    public void REQ_PRD_1_AddProduct_SavesIt()
    {
        int id = _t.NewProduct("Yogurt", 800, 25);
        var saved = _t.Products.GetAllProducts().Single(p => p.Id == id);
        Assert.Equal("Yogurt", saved.Name);
        Assert.Equal(800, saved.Price);
        Assert.Equal(25, saved.Quantity);
        Assert.Equal(DateTime.Today.AddDays(30), saved.ExpiryDate);
    }

    [Fact]
    public void REQ_PRD_1_AddProduct_RejectsEmptyName() =>
        Assert.Throws<ArgumentException>(() => _t.NewProduct(name: " "));

    [Fact]
    public void REQ_PRD_1_AddProduct_RejectsZeroPrice() =>
        Assert.Throws<ArgumentException>(() => _t.NewProduct(price: 0));

    [Fact]
    public void REQ_PRD_1_AddProduct_RejectsNegativeQuantity() =>
        Assert.Throws<ArgumentException>(() => _t.NewProduct(quantity: -1));

    [Fact]
    public void REQ_PRD_1_AddProduct_RejectsPastExpiryDate() =>
        Assert.Throws<ArgumentException>(() => _t.NewProduct(expiresInDays: -1));

    [Fact]
    public void REQ_PRD_2_UpdateProduct_ChangesData()
    {
        int id = _t.NewProduct();
        _t.Products.UpdateProduct(new Product
        {
            Id = id, Name = "Fresh Milk", Category = "Dairy", Price = 1200, Quantity = 5,
            ExpiryDate = DateTime.Today.AddDays(10)
        });
        var updated = _t.Products.GetAllProducts().Single();
        Assert.Equal("Fresh Milk", updated.Name);
        Assert.Equal(1200, updated.Price);
        Assert.Equal(5, updated.Quantity);
    }

    [Fact]
    public void REQ_PRD_2_UpdateProduct_UnknownId_Throws() =>
        Assert.Throws<InvalidOperationException>(() => _t.Products.UpdateProduct(
            new Product { Id = 999, Name = "X", Category = "Y", Price = 1, Quantity = 1 }));

    [Fact]
    public void REQ_PRD_3_DeleteProduct_RemovesIt()
    {
        int id = _t.NewProduct();
        _t.Products.DeleteProduct(id);
        Assert.Empty(_t.Products.GetAllProducts());
    }

    [Fact]
    public void REQ_PRD_3_DeleteProduct_WithSalesHistory_IsBlocked()
    {
        int id = _t.NewProduct();
        _t.Stock.SellProduct(id, 1);
        Assert.Throws<InvalidOperationException>(() => _t.Products.DeleteProduct(id));
    }

    [Fact]
    public void REQ_PRD_4_GetAllProducts_ReturnsEveryProduct()
    {
        _t.NewProduct("A");
        _t.NewProduct("B");
        Assert.Equal(2, _t.Products.GetAllProducts().Count);
    }

    [Fact]
    public void REQ_PRD_5_SearchProducts_FindsByName()
    {
        _t.NewProduct("Milk");
        _t.NewProduct("Bread");
        var result = _t.Products.SearchProducts("bre");
        Assert.Single(result);
        Assert.Equal("Bread", result[0].Name);
    }

    [Fact]
    public void REQ_PRD_5_SearchProducts_FindsByCategory()
    {
        _t.NewProduct("Milk");
        Assert.Single(_t.Products.SearchProducts("dairy"));
    }
}
