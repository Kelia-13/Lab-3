namespace ShelfLife.Models;

public class Product
{
    public const int ExpiringSoonDays = 7;

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; } = DateTime.Today.AddDays(30);

    /// <summary>Shown in the table: Expired / Expiring soon / OK.</summary>
    public string ExpiryStatus
    {
        get
        {
            var today = DateTime.Today;
            if (ExpiryDate.Date < today) return "Expired";
            if (ExpiryDate.Date <= today.AddDays(ExpiringSoonDays)) return "Expiring soon";
            return "OK";
        }
    }

    // Used by the combo box on the stock screen
    public override string ToString() =>
        $"#{Id} {Name} - {Quantity} in stock (exp {ExpiryDate:yyyy-MM-dd})";
}
