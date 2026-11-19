using ShelfLife.Controllers;
using ShelfLife.Models;

namespace ShelfLife.Views;

public class StockForm : Form
{
    private readonly StockController _controller;
    private readonly DataGridView _grid = UiHelper.MakeGrid();
    private readonly ComboBox _cmbProduct = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 420 };
    private readonly NumericUpDown _numQuantity = new() { Minimum = 1, Maximum = 100000, Value = 1, Width = 100 };

    public StockForm(StockController controller)
    {
        _controller = controller;

        Text = "ShelfLife - Stock & Sales";
        Size = new Size(960, 560);
        StartPosition = FormStartPosition.CenterParent;

        var top = new Panel { Dock = DockStyle.Top, Height = 110 };

        top.Controls.Add(new Label { Text = "Product", Location = new Point(12, 10), AutoSize = true });
        _cmbProduct.Location = new Point(12, 30);
        top.Controls.Add(_cmbProduct);

        top.Controls.Add(new Label { Text = "Quantity", Location = new Point(450, 10), AutoSize = true });
        _numQuantity.Location = new Point(450, 30);
        top.Controls.Add(_numQuantity);

        top.Controls.Add(UiHelper.MakeButton("Sell", 570, 27, 110, OnSell));
        top.Controls.Add(UiHelper.MakeButton("Restock", 690, 27, 110, OnRestock));
        top.Controls.Add(new Label { Text = "Sales history", Location = new Point(12, 82), AutoSize = true });

        Controls.Add(_grid);
        Controls.Add(top);

        Reload();
    }

    private void Reload()
    {
        _cmbProduct.DataSource = _controller.GetProducts();
        _grid.DataSource = _controller.GetSales();
        if (_grid.Columns["ProductId"] != null) _grid.Columns["ProductId"]!.Visible = false;
        if (_grid.Columns["SoldAt"] != null) _grid.Columns["SoldAt"]!.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
    }

    private void OnSell(object? sender, EventArgs e) => Run(() =>
    {
        var product = SelectedProduct();
        double total = _controller.SellProduct(product.Id, (int)_numQuantity.Value);
        Reload();
        UiHelper.Info($"Sold.\nTotal: {total:N0} RWF");
    });

    private void OnRestock(object? sender, EventArgs e) => Run(() =>
    {
        var product = SelectedProduct();
        _controller.Restock(product.Id, (int)_numQuantity.Value);
        Reload();
    });

    private Product SelectedProduct() =>
        _cmbProduct.SelectedItem as Product ?? throw new ArgumentException("Add a product first, then select it.");

    private static void Run(Action action)
    {
        try { action(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            UiHelper.Error(ex.Message);
        }
    }
}
