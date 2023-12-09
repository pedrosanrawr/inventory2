using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace inventorymanagement3
{
    public partial class inventory : Form
    {
        private int currentRowIndex = 1;
        private string currentUserIdentifier;
        private DataGridView dataGridView1;
        private List<InventoryItem> inventoryItems = new List<InventoryItem>();
        private int nextItemNumber = 1;
        private User currentUser;

        public inventory(User user)
        {
            InitializeComponent();
            currentUser = user;
            InventoryFileHandler.FilePath = "path/to/file.json";
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeDataGridView();
            LoadData();
            PopulateNumbers();
        }

        private void PopulateNumbers()
        {
            const int initialRowCount = 50;
            for (int i = 1; i <= initialRowCount; i++)
            {
                AddNumberRow();
            }

            dataGridView1.Scroll += DataGridView_Scroll;
        }

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if ((e.Type == ScrollEventType.SmallIncrement || e.Type == ScrollEventType.LargeIncrement) &&
                e.NewValue + dataGridView1.DisplayedRowCount(true) >= dataGridView1.Rows.Count - 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    AddNumberRow();
                }
            }
        }

        private void AddNumberRow()
        {
            string formattedNumber = currentRowIndex++.ToString("0000");
            dataGridView1.Rows.Add(formattedNumber, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private void InitializeDataGridView()
        {
            // Set up the DataGridView
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns to the DataGridView
            dataGridView1.Columns.Add("ItemNumber", "Item Number");
            dataGridView1.Columns.Add("ItemName", "Item");
            dataGridView1.Columns.Add("ManufacturingCost", "Manufacturing Cost");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("TotalPrice", "Total Price");

            // Set the ItemNumber column as read-only
            dataGridView1.Columns["ItemNumber"].ReadOnly = true;
            dataGridView1.Columns["ItemNumber"].Frozen = true;
            dataGridView1.Columns["TotalPrice"].ReadOnly = true;

            // Add a handler for CellEndEdit to recalculate TotalPrice when manufacturing cost or quantity changes
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Recalculate TotalPrice when manufacturing cost or quantity changes
            if (e.ColumnIndex == dataGridView1.Columns["ManufacturingCost"].Index ||
                e.ColumnIndex == dataGridView1.Columns["Quantity"].Index)
            {
                CalculateTotalPrice(e.RowIndex);
            }
        }

        private void CalculateTotalPrice(int rowIndex)
        {
            // Get the values from the cells
            string manufacturingCostValue = dataGridView1.Rows[rowIndex].Cells["ManufacturingCost"].Value?.ToString();
            string quantityValue = dataGridView1.Rows[rowIndex].Cells["Quantity"].Value?.ToString();

            // Check if both values are valid numbers
            if (double.TryParse(manufacturingCostValue, out double manufacturingCost) &&
                int.TryParse(quantityValue, out int quantity))
            {
                // Calculate TotalPrice = ManufacturingCost * Quantity
                decimal totalPrice = (decimal)(manufacturingCost * quantity);
                dataGridView1.Rows[rowIndex].Cells["TotalPrice"].Value = totalPrice;
            }
            else
            {
                // Handle the case where the values are not valid numbers
                dataGridView1.Rows[rowIndex].Cells["TotalPrice"].Value = "Invalid";
            }
        }

        private void LoadData()
        {
            // Load inventory items from the file using InventoryFileHandler
            inventoryItems = InventoryFileHandler.LoadInventoryItems(currentUserIdentifier);

            // Update nextItemNumber based on existing data
            if (inventoryItems.Any())
            {
                nextItemNumber = Convert.ToInt32(inventoryItems.Max(item => item.ItemNumber)) + 1;
            }

            // Populate DataGridView with loaded data
            PopulateDataGridView();
        }

        private void PopulateDataGridView()
        {
            // Clear existing rows
            dataGridView1.Rows.Clear();

            // Populate DataGridView with data from inventoryItems
            foreach (var item in inventoryItems)
            {
                dataGridView1.Rows.Add(item.ItemNumber, item.ItemName, item.ManufacturingCost, item.Quantity, item.TotalPrice);
            }
        }

        private void inventory_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Save data to inventoryItems with the current user's email or username
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string itemNumber = dataGridView1.Rows[i].Cells["ItemNumber"].Value.ToString();
                    string itemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();

                    // Check if the values are valid doubles
                    if (double.TryParse(dataGridView1.Rows[i].Cells["ManufacturingCost"].Value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double manufacturingCost) &&
                        int.TryParse(dataGridView1.Rows[i].Cells["Quantity"].Value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out int quantity))
                    {
                        // Check if the item is already in the list
                        var existingItem = inventoryItems.FirstOrDefault(item => item.ItemNumber == itemNumber);

                        if (existingItem != null)
                        {
                            // Update existing item only if it belongs to the current user
                            if (existingItem.UserIdentifier == currentUserIdentifier)
                            {
                                existingItem.ItemName = itemName;
                                existingItem.ManufacturingCost = manufacturingCost;
                                existingItem.Quantity = quantity;
                                existingItem.TotalPrice = (decimal)(manufacturingCost * quantity); // Update TotalPrice
                            }
                        }
                        else
                        {
                            // Add new item with the current user's email or username
                            var newItem = new InventoryItem
                            {
                                ItemNumber = itemNumber,
                                ItemName = itemName,
                                ManufacturingCost = manufacturingCost,
                                Quantity = quantity,
                                UserIdentifier = currentUserIdentifier,
                                TotalPrice = (decimal)(manufacturingCost * quantity) // Set TotalPrice
                        };
                            inventoryItems.Add(newItem);
                        }
                    }
                }

                // Save data to file
                InventoryFileHandler.SaveInventoryItems(inventoryItems, currentUserIdentifier);

                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.ToLower();

            // Loop through each row and check if the searchTerm is found in any cell
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchTerm))
                    {
                        dataGridView1.ClearSelection();
                        row.Selected = true;
                        return;
                    }
                }
            }

            // If not found, show a message
            MessageBox.Show("Item not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            items items = new items(currentUser);
            items.Show();
            this.Hide();
        }
    }
}