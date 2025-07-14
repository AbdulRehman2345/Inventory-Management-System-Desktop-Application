using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Stock_Sync
{
    public partial class StockForm : Form
    {
        public StockForm()
        {
            InitializeComponent();
        }

        // Form Load
        private void StockForm_Load(object sender, EventArgs e)
        {
            if (Session.Role == "Staff")
            {
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;

            }
            id.Focus();
        }

        private int selectedId = -1;
        String input;
        Stock stock = new Stock();

        public void Clear()
        {
            id.Text = String.Empty;
            code.Text = String.Empty;
            quantity.Text = String.Empty;
            textBox1.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox5.Text = String.Empty;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Checked = false;
        }
        private void LoadStocks()
        {
            if (!string.IsNullOrEmpty(input))
            {
                dataGridView1.DataSource = Stock.SearchStocks(input);
            }
            else
            {
                dataGridView1.DataSource = Stock.ViewAll();
            }

            // Styling
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

         // Add button
        private void insert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(id.Text) ||
             string.IsNullOrWhiteSpace(code.Text) ||
             string.IsNullOrWhiteSpace(quantity.Text) ||
             string.IsNullOrWhiteSpace(textBox1.Text) ||
             string.IsNullOrWhiteSpace(textBox3.Text) ||
             string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please fill in all required fields !");
                return;
            }
            if (!int.TryParse(id.Text, out _) ||
            !int.TryParse(quantity.Text, out _) ||
            !int.TryParse(textBox1.Text, out _) ||
            !double.TryParse(textBox3.Text, out _) ||
            !double.TryParse(textBox5.Text, out _))
            {
                MessageBox.Show("Invalid input detected. Please enter only numeric values in the fields that require numbers");
                return;
            }
            stock.Barcode_id = int.Parse(id.Text);
            stock.Stock_code = code.Text;
            stock.Quantity = int.Parse(quantity.Text);
            stock.Minimum_stock = int.Parse(textBox1.Text);
            stock.Cp_unit = double.Parse(textBox3.Text);
            stock.Sp_unit = double.Parse(textBox5.Text);
            stock.Arrival_date = dateTimePicker1.Value;
            if (dateTimePicker2.Checked)
            {
                stock.Expirey_Date = dateTimePicker2.Value;
            }
            else
            {
                stock.Expirey_Date = null;
            }
            if (stock.Add())
            {
                MessageBox.Show("Stock saved!");
                Clear();
                LoadStocks();
                id.Focus();
            }
            else
            {   
                MessageBox.Show("Stock not saved!");
            }
        }

        // Clear button
        private void button12_Click(object sender, EventArgs e)
        {
            Clear();
            id.Focus();
        }
        // View Button
        private void button11_Click(object sender, EventArgs e)
        {
            input = "";
            LoadStocks();
        }

        // Data Grid View values to Input boxes
        private void dataGridView1_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                if (selectedRow.Cells["Stock ID"].Value != DBNull.Value)
                {
                    selectedId = Convert.ToInt32(selectedRow.Cells["Stock ID"].Value);
                    stock.FetchDetailId(selectedId);

                    id.Text = stock.Barcode_id.ToString();
                    code.Text = stock.Stock_code;
                    quantity.Text = stock.Quantity.ToString();
                    textBox1.Text = stock.Minimum_stock.ToString();
                    textBox3.Text = stock.Cp_unit.ToString("F2");
                    textBox5.Text = stock.Sp_unit.ToString("F2");
                    dateTimePicker1.Value = stock.Arrival_date;

                    if (stock.Expirey_Date.HasValue)
                    {
                        dateTimePicker2.Value = stock.Expirey_Date.Value;
                        dateTimePicker2.Checked = true;
                    }
                    else
                    {
                        dateTimePicker2.Checked = false;
                    }
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
                MessageBox.Show("Please select a Stock to edit.");
                return;
            }
            if (string.IsNullOrWhiteSpace(id.Text) ||
           string.IsNullOrWhiteSpace(code.Text) ||
           string.IsNullOrWhiteSpace(quantity.Text) ||
           string.IsNullOrWhiteSpace(textBox1.Text) ||
           string.IsNullOrWhiteSpace(textBox3.Text) ||
           string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please fill in all required fields !");
                return;
            }
            if (!int.TryParse(id.Text, out _) ||
            !int.TryParse(quantity.Text, out _) ||
            !int.TryParse(textBox1.Text, out _) ||
            !double.TryParse(textBox3.Text, out _) ||
            !double.TryParse(textBox5.Text, out _))
            {
                MessageBox.Show("Invalid input detected. Please enter only numeric values in the fields that require numbers");
                return;
            }
            stock.Barcode_id = int.Parse(id.Text);
            stock.Stock_code = code.Text;
            stock.Quantity = int.Parse(quantity.Text);
            stock.Minimum_stock = int.Parse(textBox1.Text);
            stock.Cp_unit = double.Parse(textBox3.Text);
            stock.Sp_unit = double.Parse(textBox5.Text);
            stock.Arrival_date = dateTimePicker1.Value;
            if (dateTimePicker2.Checked)
            {
                stock.Expirey_Date = dateTimePicker2.Value;
            }
            else
            {
                stock.Expirey_Date = null;
            }
            if (stock.Update(selectedId))
            {
                LoadStocks();
                MessageBox.Show("Stock edited successfully.");
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
                MessageBox.Show("Please select a stock to delete.");
                return;
            }
            if (stock.Delete(selectedId))
            {
                LoadStocks();
                MessageBox.Show("Stock deleted successfully.");
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
            LoadStocks();

        }


        ////// Buttons ///////
        private void button1_Click(object sender, EventArgs e)
        {
            Buttons.OpenProductForm(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Buttons.OpenBillForm(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Buttons.OpenLoginForm(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Buttons.OpenNotificationForm(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Buttons.OpenChartForm(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Buttons.OpenReportForm(this);
        }
    }
}
