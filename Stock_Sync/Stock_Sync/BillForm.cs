using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    public partial class BillForm : Form
    {
        public BillForm()
        {
            InitializeComponent();
        }

        private void BillForm_Load(object sender, EventArgs e)
        {
            if (Session.Role == "Staff")
            {
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;

            }
            textBox11.Focus();
            textBox11.UseSystemPasswordChar = false;
            textBox11.Multiline = false;
            textBox1.UseSystemPasswordChar = false;
            textBox1.Multiline = false;
        }
        Bill bill = new Bill();
        decimal subtotal;
        private PrintDocument printDocument = new PrintDocument();
        private string printText = "";
        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // prevent ding sound
                e.SuppressKeyPress = true;

                if (long.TryParse(textBox11.Text.Trim(), out long barcode))
                {
                    bool success = bill.ScanBarcode(barcode);
                    if (success)
                    {
                        RefreshGrid();
                        UpdateSubtotal();
                    }
                    else
                    {
                        MessageBox.Show("Product not found!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid barcode!");
                }

                textBox11.Clear();
                textBox11.Focus();
            }
        }

        private void RefreshGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bill.Items.Select(i => new
            {
                Barcode = i.Product.PBarcode,
                Name = i.Product.PName,
                Quantity = i.Quantity,
                Price = i.Price,
                Total = i.Total
            }).ToList();
            // Styling
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void UpdateSubtotal()
        {
            subtotal = bill.CalculateSubtotal();
            total.Text = subtotal.ToString("C");
            sub.Text = subtotal.ToString("C");
        }

        public decimal change(decimal amount)
        {
            return amount - subtotal;
        }
        ////////// Cash Recieve and Change //////////////
        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.Text = change(decimal.Parse(textBox1.Text)).ToString("C");

            }
        }
        ////////// Save Button //////////////
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                decimal cashReceived = decimal.Parse(textBox1.Text);
                decimal subtotal = bill.CalculateSubtotal();
                decimal totalPayable = subtotal;
                decimal changeReturned = cashReceived - totalPayable;

                if (cashReceived < totalPayable)
                {
                    MessageBox.Show("Cash received is less than total payable amount!", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool saved = bill.SaveCart(cashReceived, changeReturned);
                if (saved)
                {
                    MessageBox.Show("Sale completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // RefreshUIAfterSale();
                }
                else
                {
                    MessageBox.Show("Failed to complete the sale. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid number for cash received.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string barcodeToRemove = dataGridView1.CurrentRow.Cells["Barcode"].Value?.ToString();
            if (barcodeToRemove == null)
            {
                MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var itemToRemove = bill.Items.FirstOrDefault(i => i.Product.PBarcode.ToString() == barcodeToRemove);
            if (itemToRemove != null)
            {
                bill.RemoveItem(itemToRemove);  // Remove from internal list
                RefreshGrid();                  // Rebind DataGridView to updated data source
                UpdateSubtotal();               // Update totals in UI
            }
            else
            {
                MessageBox.Show("Item not found in cart.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bill.Clear();        // Clear the internal list in Bill class
            RefreshGrid();       // Refresh the DataGridView (will show empty)

            // Clear the textboxes related to subtotal, total, cash received, change
            sub.Text = "";
            total.Text = "";
            textBox1.Clear();    // Cash received textbox
            textBox2.Clear();    // Change textbox
            textBox11.Clear();   // Barcode input textbox

            textBox11.Focus();
        }

        private void insert_Click(object sender, EventArgs e)
        {
            // Build the print string
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("        Stock Sync Billing Receipt");
            sb.AppendLine("============================================");
            sb.AppendLine("Barcode       Name        Qty   Price   Total");

            foreach (var item in bill.Items)
            {
                sb.AppendLine($"{item.Product.PBarcode}   {item.Product.PName,-10} {item.Quantity,3}   {item.Price,6:C}   {item.Total,6:C}");
            }

            sb.AppendLine("============================================");
            sb.AppendLine($"Subtotal:      {sub.Text}");
            sb.AppendLine($"Total Payable: {total.Text}");
            sb.AppendLine($"Cash Received: {textBox1.Text}");
            sb.AppendLine($"Change:        {textBox2.Text}");
            sb.AppendLine("============================================");
            sb.AppendLine($"Date: {DateTime.Now}");

            printText = sb.ToString();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Show print dialog (optional)
            PrintDialog dialog = new PrintDialog();
            dialog.Document = printDocument;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Consolas", 10);
            e.Graphics.DrawString(printText, font, Brushes.Black, new PointF(10, 10));
        }
        ////// Buttons ///////
        private void button1_Click(object sender, EventArgs e)
        {
            Buttons.OpenProductForm(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Buttons.OpenStockForm(this);
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

        private void button8_Click(object sender, EventArgs e)
        {
            Buttons.OpenLoginForm(this);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
} 


