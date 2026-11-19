namespace ShelfLife.Models;

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
    public DateTime SoldAt { get; set; }
}
