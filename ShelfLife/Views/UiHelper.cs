using ShelfLife.Models;

namespace ShelfLife.Views;

/// <summary>Small helpers so the forms stay short and look the same.</summary>
public static class UiHelper
{
    public static void Error(string message) =>
        MessageBox.Show(message, "ShelfLife", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void Info(string message) =>
        MessageBox.Show(message, "ShelfLife", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string message) =>
        MessageBox.Show(message, "ShelfLife", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    public static Button MakeButton(string text, int x, int y, int width, EventHandler onClick)
    {
        var button = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 32) };
        button.Click += onClick;
        return button;
    }

    public static void AddField(Control parent, string label, Control input, int y)
    {
        parent.Controls.Add(new Label { Text = label, Location = new Point(12, y), AutoSize = true });
        input.Location = new Point(12, y + 20);
        input.Width = 250;
        parent.Controls.Add(input);
    }

    public static DataGridView MakeGrid() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false,
        BackgroundColor = SystemColors.Window
    };

    /// <summary>Colours product rows: red = expired, yellow = expiring soon. Also formats the date column.</summary>
    public static void StyleProductGrid(DataGridView grid)
    {
        if (grid.Columns["ExpiryDate"] is DataGridViewColumn dateColumn)
            dateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";

        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row.DataBoundItem is not Product p) continue;
            row.DefaultCellStyle.BackColor = p.ExpiryStatus switch
            {
                "Expired" => Color.MistyRose,
                "Expiring soon" => Color.LemonChiffon,
                _ => SystemColors.Window
            };
        }
    }
}
