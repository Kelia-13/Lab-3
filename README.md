# ShelfLife – Supermarket Expiry & Stock Management (Lab 3)

A Windows Forms desktop application (C# / .NET 8) with a SQLite database, built with the **MVC architecture**.
Staff manage products, sell and restock them, and get alerts about products that are expired or about to expire.

## How to run
1. Install **Visual Studio 2022** with the *.NET desktop development* workload (Windows).
2. Open `ShelfLife.sln`.
3. Set **ShelfLife** as the startup project and press **F5**.
   The database file `shelflife.db` is created automatically on first run (NuGet restores SQLite by itself).

## Run the tests
Visual Studio → *Test* → *Run All Tests*, or in a terminal: `dotnet test`

## Architecture (MVC)
| Layer | Folder | Job |
|-------|--------|-----|
| Model | `Models/`, `Data/` | Data classes and the repositories that run the SQL |
| Controller | `Controllers/` | Validation and business rules |
| View | `Views/` | WinForms screens (no SQL, no rules) |

Views call Controllers → Controllers call Repositories → Repositories talk to the database.

## Requirements and where they are implemented / tested
| ID | Feature | Method |
|----|---------|--------|
| REQ-PRD-1 | Add product | `ProductController.AddProduct` |
| REQ-PRD-2 | Update product | `ProductController.UpdateProduct` |
| REQ-PRD-3 | Delete product | `ProductController.DeleteProduct` |
| REQ-PRD-4 | View all products | `ProductController.GetAllProducts` |
| REQ-PRD-5 | Search products | `ProductController.SearchProducts` |
| REQ-STK-1 | Restock a product | `StockController.Restock` |
| REQ-STK-2 | Sell a product | `StockController.SellProduct` |
| REQ-STK-3 | View sales history | `StockController.GetSales` |
| REQ-EXP-1 | List expired products | `ExpiryController.GetExpiredProducts` |
| REQ-EXP-2 | List products expiring soon | `ExpiryController.GetExpiringSoon` |
| REQ-EXP-3 | Remove expired stock | `ExpiryController.RemoveExpiredStock` |

Every requirement has at least one test in `ShelfLife.Tests` (test names start with the requirement ID).

**Rules:** expired products cannot be sold or restocked, stock never goes below 0, and products with sales history cannot be deleted.
In the product table, red rows are expired and yellow rows expire within 7 days.

Author: HABANZINTWALI Ihirwe Kelia – ID 26331
