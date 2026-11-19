using ShelfLife.Controllers;

namespace ShelfLife.Views;

public class MainForm : Form
{
    private readonly ProductController _products;
    private readonly StockController _stock;
    private readonly ExpiryController _expiry;

    public MainForm(ProductController products, StockController stock, ExpiryController expiry)
    {
        _products = products;
        _stock = stock;
        _expiry = expiry;

        Text = "ShelfLife - Supermarket Expiry & Stock Management";
        Size = new Size(460, 440);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        Controls.Add(new Label
        {
            Text = "ShelfLife",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(140, 25)
        });
        Controls.Add(new Label
        {
            Text = "Supermarket Expiry & Stock Management",
            AutoSize = true,
            Location = new Point(110, 75)
        });

        AddMenuButton("Manage Products", 120, () => new ProductForm(_products));
        AddMenuButton("Stock & Sales", 175, () => new StockForm(_stock));
        AddMenuButton("Expiry Alerts", 230, () => new ExpiryForm(_expiry));
        Controls.Add(UiHelper.MakeButton("Exit", 90, 305, 270, (_, _) => Close()));
    }

    private void AddMenuButton(string text, int y, Func<Form> createForm)
    {
        var button = UiHelper.MakeButton(text, 90, y, 270, (_, _) =>
        {
            using var form = createForm();
            form.ShowDialog(this);
        });
        button.Height = 42;
        Controls.Add(button);
    }
}
