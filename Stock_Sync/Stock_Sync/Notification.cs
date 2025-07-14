using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    internal class Notification
    {

        public static void ShowLowStockAlerts(RichTextBox rtb)
        {
            rtb.SelectionAlignment = HorizontalAlignment.Center;
            rtb.SelectionFont = new Font("Arial", 16, FontStyle.Bold | FontStyle.Underline );
            rtb.SelectionColor = Color.FromArgb(25, 45, 55);
            rtb.AppendText("\nLOW STOCK \n\n");

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT p.PName, s.quantity, s.minimum_stock 
            FROM Products p 
            INNER JOIN Stocks s ON p.ID = s.barcode_id 
            WHERE s.quantity < s.minimum_stock";

                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        rtb.SelectionAlignment = HorizontalAlignment.Left;
                        rtb.SelectionFont = new Font("Arial", 12, FontStyle.Italic);
                        rtb.SelectionColor = Color.Gray;
                        rtb.AppendText(" All products have sufficient stock.\n\n");
                        return;
                    }
                    rtb.SelectionAlignment = HorizontalAlignment.Left;

                    while (reader.Read())
                    {
                        string name = reader["PName"].ToString();
                        int qty = Convert.ToInt32(reader["quantity"]);
                        rtb.SelectionFont = new Font("Arial", 14, FontStyle.Italic);
                        rtb.SelectionColor = Color.Black;
                        rtb.AppendText($"    - {name} — only {qty} left in stock\n\n");
                    }

                    rtb.AppendText("\n"); 
                }
            }
        }



        public static void ShowExpiredProductAlerts(RichTextBox rtb)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                rtb.SelectionAlignment = HorizontalAlignment.Center;
                rtb.SelectionFont = new Font("Arial", 16, FontStyle.Bold | FontStyle.Underline);
                rtb.SelectionColor = Color.FromArgb(37, 63, 75);
                rtb.AppendText("\nEXPIRED PRODUCT STOCKS \n\n");
                conn.Open();
                string query = "SELECT p.PName AS ProductName, s.expiry_date FROM Products p INNER JOIN Stocks s ON p.ID = s.barcode_id WHERE s.expiry_date <= GETDATE(); ";
                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        rtb.SelectionAlignment = HorizontalAlignment.Left;
                        rtb.SelectionFont = new Font("Arial", 12, FontStyle.Italic);
                        rtb.SelectionColor = Color.Gray;
                        rtb.AppendText("No expired Products.");
                    }
                    rtb.SelectionAlignment = HorizontalAlignment.Left;
                    while (reader.Read())
                    {
                        string name = reader["ProductName"].ToString();
                        DateTime expiry = Convert.ToDateTime(reader["expiry_date"]);

                        rtb.SelectionFont = new Font("Arial", 14, FontStyle.Italic);
                        rtb.SelectionColor = Color.Black;
                        rtb.AppendText($"   - {name} has expired on {expiry:dd MMM yyyy}\n\n");
                    }
                    rtb.AppendText("\n"); 
                }
            }
        }
        public static void ShowNeverSoldItems(RichTextBox rtb)
        {
            rtb.SelectionAlignment = HorizontalAlignment.Center;
            rtb.SelectionFont = new Font("Arial", 16, FontStyle.Bold | FontStyle.Underline);
            rtb.SelectionColor = Color.FromArgb(25, 45, 55);
            rtb.AppendText("\nNEVER SOLD ITEMS \n\n");

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT p.PName 
            FROM Products p
            LEFT JOIN Stocks s ON p.ID = s.barcode_id
            WHERE s.barcode_id IS NULL";

                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        rtb.SelectionAlignment = HorizontalAlignment.Left;
                        rtb.SelectionFont = new Font("Segoe UI", 12, FontStyle.Italic);
                        rtb.SelectionColor = Color.Gray;
                        rtb.AppendText(" All products have been sold at least once.\n\n");
                        return;
                    }

                    rtb.SelectionAlignment = HorizontalAlignment.Left;

                    while (reader.Read())
                    {
                        string name = reader["PName"].ToString();
                        rtb.SelectionFont = new Font("Arial", 14, FontStyle.Italic);
                        rtb.SelectionColor = Color.Black;
                        rtb.AppendText($"    - {name} has never been sold \n\n");
                    }

                    rtb.AppendText("\n");
                }
            }
        }

    }
}
