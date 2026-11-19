using System.Globalization;
using ShelfLife.Controllers;
using ShelfLife.Models;

namespace ShelfLife.Views;

public class ProductForm : Form
{
    private readonly ProductController _controller;
    private readonly DataGridView _grid = UiHelper.MakeGrid();
    private readonly TextBox _txtName = new();
    private readonly ComboBox _cmbCategory = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private readonly TextBox _txtPrice = new();
    private readonly NumericUpDown _numQuantity = new() { Minimum = 0, Maximum = 1000000 };
    private readonly DateTimePicker _dtpExpiry = new() { Format = DateTimePickerFormat.Short };
    private readonly TextBox _txtSearch = new() { Width = 220 };
    private int _selectedId;

    public ProductForm(ProductController controller)
    {
        _controller = controller;

        Text = "ShelfLife - Products";
        Size = new Size(1000, 560);
        StartPosition = FormStartPosition.CenterParent;

        _cmbCategory.Items.AddRange(new object[]
            { "Dairy", "Bakery", "Beverages", "Meat", "Produce", "Frozen", "Snacks", "Household" });

        var searchBar = new Panel { Dock = DockStyle.Top, Height = 44 };
        _txtSearch.Location = new Point(10, 10);
        searchBar.Controls.Add(_txtSearch);
        searchBar.Controls.Add(UiHelper.MakeButton("Search", 240, 6, 80, (_, _) => LoadGrid(_txtSearch.Text)));
        searchBar.Controls.Add(UiHelper.MakeButton("Show all", 328, 6, 80, (_, _) => { _txtSearch.Clear(); LoadGrid(""); }));

        var side = new Panel { Dock = DockStyle.Left, Width = 280 };
        UiHelper.AddField(side, "Product name", _txtName, 10);
        UiHelper.AddField(side, "Category", _cmbCategory, 65);
        UiHelper.AddField(side, "Price (RWF)", _txtPrice, 120);
        UiHelper.AddField(side, "Quantity in stock", _numQuantity, 175);
        UiHelper.AddField(side, "Expiry date", _dtpExpiry, 230);
        side.Controls.Add(UiHelper.MakeButton("Add", 12, 305, 120, OnAdd));
        side.Controls.Add(UiHelper.MakeButton("Update", 142, 305, 120, OnUpdate));
        side.Controls.Add(UiHelper.MakeButton("Delete", 12, 345, 120, OnDelete));
        side.Controls.Add(UiHelper.MakeButton("Clear", 142, 345, 120, (_, _) => ClearFields()));

        // Fill control first, then the docked panels (WinForms docking order)
        Controls.Add(_grid);
        Controls.Add(searchBar);
        Controls.Add(side);

        _grid.SelectionChanged += OnRowSelected;
        _grid.DataBindingComplete += (_, _) => UiHelper.StyleProductGrid(_grid);
        LoadGrid("");
    }

    private void LoadGrid(string search)
    {
        _grid.DataSource = _controller.SearchProducts(search);
        ClearFields();   // WinForms auto-selects row 1; start with an empty form
    }

    private void OnRowSelected(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0) return;
        if (_grid.CurrentRow?.DataBoundItem is not Product p) return;
        _selectedId = p.Id;
        _txtName.Text = p.Name;
        _cmbCategory.Text = p.Category;
        _txtPrice.Text = p.Price.ToString(CultureInfo.InvariantCulture);
        _numQuantity.Value = Math.Min(p.Quantity, (int)_numQuantity.Maximum);
        _dtpExpiry.Value = p.ExpiryDate;
    }

    private void ClearFields()
    {
        _selectedId = 0;
        _txtName.Clear();
        _cmbCategory.Text = "";
        _txtPrice.Clear();
        _numQuantity.Value = 0;
        _dtpExpiry.Value = DateTime.Today.AddDays(30);
        _grid.ClearSelection();
    }

    private Product ReadFields()
    {
        if (!double.TryParse(_txtPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            throw new ArgumentException("Price must be a number.");
        return new Product
        {
            Id = _selectedId,
            Name = _txtName.Text,
            Category = _cmbCategory.Text,
            Price = price,
            Quantity = (int)_numQuantity.Value,
            ExpiryDate = _dtpExpiry.Value.Date
        };
    }

    private void OnAdd(object? sender, EventArgs e) => Run(() =>
    {
        _controller.AddProduct(ReadFields());
        LoadGrid(_txtSearch.Text);
    });

    private void OnUpdate(object? sender, EventArgs e) => Run(() =>
    {
        if (_selectedId == 0) throw new ArgumentException("Select a product in the table first.");
        _controller.UpdateProduct(ReadFields());
        LoadGrid(_txtSearch.Text);
    });

    private void OnDelete(object? sender, EventArgs e) => Run(() =>
    {
        if (_selectedId == 0) throw new ArgumentException("Select a product in the table first.");
        if (!UiHelper.Confirm("Delete the selected product?")) return;
        _controller.DeleteProduct(_selectedId);
        LoadGrid(_txtSearch.Text);
    });

    private static void Run(Action action)
    {
        try { action(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            UiHelper.Error(ex.Message);
        }
    }
}
