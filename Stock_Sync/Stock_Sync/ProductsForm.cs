using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Stock_Sync
{
    public partial class ProductsForm : Form
    {
        public ProductsForm()
        {
            InitializeComponent();
        }

        // Form Load
        private void ProductsForm_Load(object sender, EventArgs e)
        {
            if (Session.Role == "Staff")
            {
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;

            }
            barcode.Focus();
            barcode.UseSystemPasswordChar = false;
            barcode.Multiline = false;
            textBox11.UseSystemPasswordChar = false;
            textBox11.Multiline = false;

        }
        private int selectedId = -1;
        String input;
        Product product = new Product();

        public void Clear()
        {
            barcode.Text = String.Empty;
            name.Text = String.Empty;
            category.Text = String.Empty;
            status.SelectedIndex = -1;
        }
        private void LoadProducts()
        {
            if (!string.IsNullOrEmpty(input))
            {
                dataGridView1.DataSource = Product.SearchProducts(input);
            }
            else
            {
                dataGridView1.DataSource = Product.ViewAll();
            }

            // Styling
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Add button
        private void insert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(barcode.Text) ||
            string.IsNullOrWhiteSpace(name.Text) ||
            string.IsNullOrWhiteSpace(category.Text) ||
            string.IsNullOrWhiteSpace(status.Text))
            {
                MessageBox.Show("Please fill in all required fields !");
                return;
            }
            if (!barcode.Text.All(char.IsDigit))
            {
                MessageBox.Show("Invalid Input Detected. Please enter a valid numeric barcode");
                return;
            }
            product.PBarcode = long.Parse(barcode.Text);
            product.PName = name.Text;
            product.PCategory = category.Text;
            product.PStatus = status.Text;
            if (product.Add())
            {
                MessageBox.Show("Product saved!");
                Clear();
                LoadProducts();
                barcode.Focus();
            }
            else
            {
                MessageBox.Show("Product not saved!");
            }
        }

        // Clear button
        private void button12_Click(object sender, EventArgs e)
        {
            Clear();
            barcode.Focus();
        }
        // View Button
        private void button11_Click(object sender, EventArgs e)
        {
            input = "";
            LoadProducts();
        }

        //Data Grid View values to Input boxes
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                if (selectedRow.Cells["id"].Value != DBNull.Value)
                {
                    selectedId = Convert.ToInt32(selectedRow.Cells["id"].Value);
                    product.FetchDetailId(selectedId);
                    barcode.Text = product.PBarcode.ToString();
                    name.Text = product.PName;
                    category.Text = product.PCategory;
                    status.SelectedItem = product.PStatus;
                }
                else
                {
                    MessageBox.Show("This row doesn't contain valid data.");
                }
            }
        }
        // Update button
        private void button9_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a product to edit.");
                return;
            }
            if (string.IsNullOrWhiteSpace(barcode.Text) ||
            string.IsNullOrWhiteSpace(name.Text) ||
            string.IsNullOrWhiteSpace(category.Text) ||
            string.IsNullOrWhiteSpace(status.Text))
            {
                MessageBox.Show("Please fill in all required fields !");
                return;
            }
            if (!barcode.Text.All(char.IsDigit))
            {
                MessageBox.Show("Invalid Input Detected. Please enter a valid numeric barcode");
                return;
            }
            product.PBarcode = long.Parse(barcode.Text);
            product.PName = name.Text;
            product.PCategory = category.Text;
            product.PStatus = status.SelectedItem.ToString();
            if (product.Update(selectedId))
            {
                LoadProducts();
                MessageBox.Show("Product edited successfully.");
                Clear();
                selectedId = -1;
            }
            else
            {
                MessageBox.Show("edit failed.");
            }
        }
        // Delete Button
        private void button10_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }
            if (product.Delete(selectedId))
            {
                LoadProducts();
                MessageBox.Show("Product deleted successfully.");
                Clear();
                selectedId = -1;
            }
            else
            {
                MessageBox.Show("delete failed.");
            }
        }
        // View Button
        private void button13_Click(object sender, EventArgs e)
        {
            input = textBox11.Text.Trim();
            LoadProducts();

        }
        ////// Buttons ///////
        private void button2_Click(object sender, EventArgs e)
        {
            Buttons.OpenStockForm(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Buttons.OpenBillForm(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Buttons.OpenChartForm(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Buttons.OpenReportForm(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Buttons.OpenNotificationForm(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Buttons.OpenLoginForm(this);
        }
    }
}
