using ShelfLife.Data;
using ShelfLife.Models;

namespace ShelfLife.Controllers;

/// <summary>Feature: Expiry Alerts (REQ-EXP-1 to REQ-EXP-3)</summary>
public class ExpiryController
{
    private readonly ProductRepository _repo;

    public ExpiryController(ProductRepository repo) => _repo = repo;

    /// <summary>REQ-EXP-1: List products in stock that are already expired.</summary>
    public List<Product> GetExpiredProducts(DateTime? today = null) =>
        _repo.GetExpired((today ?? DateTime.Today).Date);

    /// <summary>REQ-EXP-2: List products in stock that expire within the next N days.</summary>
    public List<Product> GetExpiringSoon(int days, DateTime? today = null)
    {
        if (days < 1) throw new ArgumentException("Number of days must be at least 1.");
        var start = (today ?? DateTime.Today).Date;
        return _repo.GetExpiringBetween(start, start.AddDays(days));
    }

    /// <summary>REQ-EXP-3: Remove expired stock (set quantity to 0). Returns how many products were cleared.</summary>
    public int RemoveExpiredStock(DateTime? today = null) =>
        _repo.ClearExpiredStock((today ?? DateTime.Today).Date);
}
