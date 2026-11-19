namespace ShelfLife.Tests;

public class StockTests : IDisposable
{
    private readonly TestDb _t = new();
    public void Dispose() => _t.Dispose();

    [Fact]
    public void REQ_STK_1_Restock_AddsToQuantity()
    {
        int id = _t.NewProduct(quantity: 10);
        _t.Stock.Restock(id, 5);
        Assert.Equal(15, _t.Products.GetAllProducts().Single().Quantity);
    }

    [Fact]
    public void REQ_STK_1_Restock_RejectsZeroQuantity()
    {
        int id = _t.NewProduct();
        Assert.Throws<ArgumentException>(() => _t.Stock.Restock(id, 0));
    }

    [Fact]
    public void REQ_STK_1_Restock_ExpiredProduct_Throws()
    {
        int id = _t.NewProduct();
        _t.MakeExpired(id);
        Assert.Throws<InvalidOperationException>(() => _t.Stock.Restock(id, 5));
    }

    [Fact]
    public void REQ_STK_2_SellProduct_ReducesStockAndReturnsTotal()
    {
        int id = _t.NewProduct(price: 1000, quantity: 10);
        double total = _t.Stock.SellProduct(id, 3);
        Assert.Equal(3000, total);
        Assert.Equal(7, _t.Products.GetAllProducts().Single().Quantity);
    }

    [Fact]
    public void REQ_STK_2_SellProduct_MoreThanStock_Throws()
    {
        int id = _t.NewProduct(quantity: 2);
        Assert.Throws<InvalidOperationException>(() => _t.Stock.SellProduct(id, 3));
        Assert.Equal(2, _t.Products.GetAllProducts().Single().Quantity);
    }

    [Fact]
    public void REQ_STK_2_SellProduct_ExpiredProduct_Throws()
    {
        int id = _t.NewProduct();
        _t.MakeExpired(id);
        Assert.Throws<InvalidOperationException>(() => _t.Stock.SellProduct(id, 1));
    }

    [Fact]
    public void REQ_STK_2_SellProduct_RejectsZeroQuantity()
    {
        int id = _t.NewProduct();
        Assert.Throws<ArgumentException>(() => _t.Stock.SellProduct(id, 0));
    }

    [Fact]
    public void REQ_STK_3_GetSales_ListsSalesNewestFirst()
    {
        int id = _t.NewProduct(price: 500, quantity: 10);
        _t.Stock.SellProduct(id, 1);
        _t.Stock.SellProduct(id, 2);

        var sales = _t.Stock.GetSales();

        Assert.Equal(2, sales.Count);
        Assert.Equal(2, sales[0].Quantity);       // newest first
        Assert.Equal(1000, sales[0].TotalPrice);
        Assert.Equal("Milk", sales[0].ProductName);
    }
}
