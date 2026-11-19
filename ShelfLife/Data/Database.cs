using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ShelfLife.Data;

/// <summary>Creates the SQLite database file and tables, and opens connections.</summary>
public class Database
{
    private readonly string _connectionString;

    public Database(string? path = null)
    {
        path ??= Path.Combine(AppContext.BaseDirectory, "shelflife.db");
        _connectionString = new SqliteConnectionStringBuilder { DataSource = path }.ToString();
        Initialize();
    }

    /// <summary>Dates are stored as yyyy-MM-dd text so they can be compared in SQL.</summary>
    public static string FormatDate(DateTime date) =>
        date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static DateTime ParseDate(string text) =>
        DateTime.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    public SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();
        return connection;
    }

    private void Initialize()
    {
        using var connection = Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                Name       TEXT NOT NULL,
                Category   TEXT NOT NULL,
                Price      REAL NOT NULL,
                Quantity   INTEGER NOT NULL,
                ExpiryDate TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Sales (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId  INTEGER NOT NULL REFERENCES Products(Id),
                Quantity   INTEGER NOT NULL,
                TotalPrice REAL NOT NULL,
                SoldAt     TEXT NOT NULL
            );";
        cmd.ExecuteNonQuery();
    }
}
