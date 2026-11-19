using Microsoft.Data.Sqlite;
using ShelfLife.Models;

namespace ShelfLife.Data;

public class ProductRepository
{
    private const string Columns = "Id, Name, Category, Price, Quantity, ExpiryDate";
    private readonly Database _db;

    public ProductRepository(Database db) => _db = db;

    public int Add(Product p)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO Products (Name, Category, Price, Quantity, ExpiryDate)
                            VALUES ($name, $category, $price, $qty, $expiry);
                            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$name", p.Name);
        cmd.Parameters.AddWithValue("$category", p.Category);
        cmd.Parameters.AddWithValue("$price", p.Price);
        cmd.Parameters.AddWithValue("$qty", p.Quantity);
        cmd.Parameters.AddWithValue("$expiry", Database.FormatDate(p.ExpiryDate));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Product p)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"UPDATE Products
                            SET Name=$name, Category=$category, Price=$price, Quantity=$qty, ExpiryDate=$expiry
                            WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", p.Id);
        cmd.Parameters.AddWithValue("$name", p.Name);
        cmd.Parameters.AddWithValue("$category", p.Category);
        cmd.Parameters.AddWithValue("$price", p.Price);
        cmd.Parameters.AddWithValue("$qty", p.Quantity);
        cmd.Parameters.AddWithValue("$expiry", Database.FormatDate(p.ExpiryDate));
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "DELETE FROM Products WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public Product? GetById(int id)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columns} FROM Products WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    public List<Product> GetAll()
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {Columns} FROM Products ORDER BY Id";
        return ReadList(cmd);
    }

    public List<Product> Search(string text)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $@"SELECT {Columns} FROM Products
                             WHERE Name LIKE $q OR Category LIKE $q
                             ORDER BY Id";
        cmd.Parameters.AddWithValue("$q", $"%{text}%");
        return ReadList(cmd);
    }

    /// <summary>Products that are in stock and whose expiry date is before the given day.</summary>
    public List<Product> GetExpired(DateTime today)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $@"SELECT {Columns} FROM Products
                             WHERE ExpiryDate < $today AND Quantity > 0
                             ORDER BY ExpiryDate";
        cmd.Parameters.AddWithValue("$today", Database.FormatDate(today));
        return ReadList(cmd);
    }

    /// <summary>Products in stock that expire from today up to the given last day.</summary>
    public List<Product> GetExpiringBetween(DateTime today, DateTime lastDay)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = $@"SELECT {Columns} FROM Products
                             WHERE ExpiryDate >= $today AND ExpiryDate <= $last AND Quantity > 0
                             ORDER BY ExpiryDate";
        cmd.Parameters.AddWithValue("$today", Database.FormatDate(today));
        cmd.Parameters.AddWithValue("$last", Database.FormatDate(lastDay));
        return ReadList(cmd);
    }

    public void SetQuantity(int id, int quantity)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "UPDATE Products SET Quantity=$qty WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$qty", quantity);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Sets the stock of every expired product to 0 and returns how many products were changed.</summary>
    public int ClearExpiredStock(DateTime today)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "UPDATE Products SET Quantity=0 WHERE ExpiryDate < $today AND Quantity > 0";
        cmd.Parameters.AddWithValue("$today", Database.FormatDate(today));
        return cmd.ExecuteNonQuery();
    }

    public bool HasSales(int id)
    {
        using var c = _db.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Sales WHERE ProductId=$id";
        cmd.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    private static List<Product> ReadList(SqliteCommand cmd)
    {
        var list = new List<Product>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    private static Product Map(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Name = r.GetString(1),
        Category = r.GetString(2),
        Price = r.GetDouble(3),
        Quantity = r.GetInt32(4),
        ExpiryDate = Database.ParseDate(r.GetString(5))
    };
}
