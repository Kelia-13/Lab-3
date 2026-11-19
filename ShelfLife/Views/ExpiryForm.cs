using ShelfLife.Controllers;

namespace ShelfLife.Views;

public class ExpiryForm : Form
{
    private readonly ExpiryController _controller;
    private readonly DataGridView _grid = UiHelper.MakeGrid();
    private readonly NumericUpDown _numDays = new() { Minimum = 1, Maximum = 365, Value = 7, Width = 60 };
    private readonly Label _lblInfo = new() { AutoSize = true, Location = new Point(12, 70) };

    public ExpiryForm(ExpiryController controller)
    {
        _controller = controller;

        Text = "ShelfLife - Expiry Alerts";
        Size = new Size(960, 540);
        StartPosition = FormStartPosition.CenterParent;

        var top = new Panel { Dock = DockStyle.Top, Height = 100 };
        top.Controls.Add(UiHelper.MakeButton("Show expired products", 12, 15, 190, (_, _) => ShowExpired()));

        top.Controls.Add(new Label { Text = "Expiring within", Location = new Point(230, 22), AutoSize = true });
        _numDays.Location = new Point(325, 18);
        top.Controls.Add(_numDays);
        top.Controls.Add(new Label { Text = "days", Location = new Point(390, 22), AutoSize = true });
        top.Controls.Add(UiHelper.MakeButton("Show", 430, 15, 80, (_, _) => ShowExpiringSoon()));

        top.Controls.Add(UiHelper.MakeButton("Remove expired stock", 600, 15, 190, OnRemoveExpired));
        top.Controls.Add(_lblInfo);

        Controls.Add(_grid);
        Controls.Add(top);

        _grid.DataBindingComplete += (_, _) => UiHelper.StyleProductGrid(_grid);
        ShowExpired();
    }

    private void ShowExpired()
    {
        var list = _controller.GetExpiredProducts();
        _grid.DataSource = list;
        _lblInfo.Text = $"{list.Count} expired product(s) still in stock.";
    }

    private void ShowExpiringSoon()
    {
        int days = (int)_numDays.Value;
        var list = _controller.GetExpiringSoon(days);
        _grid.DataSource = list;
        _lblInfo.Text = $"{list.Count} product(s) expiring within {days} day(s).";
    }

    private void OnRemoveExpired(object? sender, EventArgs e)
    {
        if (!UiHelper.Confirm("Set the stock of all expired products to 0?")) return;
        int count = _controller.RemoveExpiredStock();
        ShowExpired();
        UiHelper.Info($"{count} product(s) cleared.");
    }
}
