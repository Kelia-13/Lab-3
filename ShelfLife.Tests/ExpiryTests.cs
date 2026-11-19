namespace ShelfLife.Tests;

public class ExpiryTests : IDisposable
{
    private readonly TestDb _t = new();
    public void Dispose() => _t.Dispose();

    [Fact]
    public void REQ_EXP_1_GetExpiredProducts_ListsOnlyExpired()
    {
        _t.NewProduct("Fresh", expiresInDays: 30);
        int old = _t.NewProduct("Old", expiresInDays: 5);

        // look at the shop 10 days from now: "Old" is expired, "Fresh" is not
        var expired = _t.Expiry.GetExpiredProducts(DateTime.Today.AddDays(10));

        Assert.Single(expired);
        Assert.Equal(old, expired[0].Id);
    }

    [Fact]
    public void REQ_EXP_1_GetExpiredProducts_IgnoresEmptyShelves()
    {
        _t.NewProduct("Empty", quantity: 0, expiresInDays: 1);
        Assert.Empty(_t.Expiry.GetExpiredProducts(DateTime.Today.AddDays(10)));
    }

    [Fact]
    public void REQ_EXP_2_GetExpiringSoon_ListsOnlyProductsInsideTheWindow()
    {
        int soon = _t.NewProduct("Soon", expiresInDays: 3);
        _t.NewProduct("Later", expiresInDays: 30);

        var result = _t.Expiry.GetExpiringSoon(7);

        Assert.Single(result);
        Assert.Equal(soon, result[0].Id);
    }

    [Fact]
    public void REQ_EXP_2_GetExpiringSoon_RejectsZeroDays() =>
        Assert.Throws<ArgumentException>(() => _t.Expiry.GetExpiringSoon(0));

    [Fact]
    public void REQ_EXP_3_RemoveExpiredStock_SetsQuantityToZero()
    {
        _t.NewProduct("Fresh", quantity: 10, expiresInDays: 30);
        int old = _t.NewProduct("Old", quantity: 8, expiresInDays: 5);

        int cleared = _t.Expiry.RemoveExpiredStock(DateTime.Today.AddDays(10));

        Assert.Equal(1, cleared);
        var products = _t.Products.GetAllProducts();
        Assert.Equal(0, products.Single(p => p.Id == old).Quantity);
        Assert.Equal(10, products.Single(p => p.Name == "Fresh").Quantity);
    }
}
