using System.Globalization;
using Microsoft.Data.Sqlite;
using ShelfLife.Models;

namespace ShelfLife.Data;

public class SaleRepository
{
    private readonly Database _db;

    public SaleRepository(Database db) => _db = db;

    /// <summary>
    /// Saves a sale and reduces the product stock in one transaction.
    /// Throws if there is not enough stock.
    /// </summary>
    public int Record(int productId, int quantity, double total, DateTime soldAt)
    {
        using var c = _db.Open();
        using var tx = c.BeginTransaction();

        using var stock = c.CreateCommand();
        stock.Transaction = tx;
        stock.CommandText = "UPDATE Products SET Quantity = Quantity - $qty WHERE Id=$id AND Quantity >= $qty";
        stock.Parameters.AddWithValue("$qty", quantity);
        stock.Parameters.AddWithValue("$id", productId);
        if (stock.ExecuteNonQuery() == 0)
            throw new InvalidOperationException("Not enough stock for this sale.");

        using var insert = c.CreateCommand();
        insert.Transaction = tx;
        insert.CommandText = @"INSERT INTO Sales (ProductId, Quantity, TotalPrice, SoldAt)
                               VALUES ($id, $qty, $total, $at);
                               SELECT last_insert_rowid();";
        insert.Parameters.AddWithValue("$id", productId);
        insert.Parameters.AddWithValue("$qty", quantity);
        insert.Parameters.AddWithValue("$total", total);
        insert.Parameters.AddWithValue("$at", soldAt.ToString("o"));
        int saleId = Convert.ToInt32(insert.ExecuteScalar());

        tx.Commit();
        return saleId;
    }

    public List<Sale> GetAll()
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"SELECT s.Id, s.ProductId, p.Name, s.Quantity, s.TotalPrice, s.SoldAt
                            FROM Sales s JOIN Products p ON p.Id = s.ProductId
                            ORDER BY s.Id DESC";
        var list = new List<Sale>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    private static Sale Map(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        ProductId = r.GetInt32(1),
        ProductName = r.GetString(2),
        Quantity = r.GetInt32(3),
        TotalPrice = r.GetDouble(4),
        SoldAt = DateTime.Parse(r.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
    };
}
