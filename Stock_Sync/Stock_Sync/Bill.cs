using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    internal class Bill
    {
        private List<BillItem> items = new List<BillItem>();

        public IReadOnlyList<BillItem> Items => items.AsReadOnly();

        public bool ScanBarcode(long barcode)
        {
            var product = GetProductByBarcode(barcode);
            if (product == null)
                return false;

            int availableStock = GetAvailableStock(barcode);
            if (availableStock <= 0)
            {
                MessageBox.Show($"Product {product.PName} is out of stock!", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var existingItem = items.FirstOrDefault(i => i.Product.PBarcode == barcode);
            if (existingItem != null)
            {
                if (existingItem.Quantity + 1 > availableStock)
                {
                    MessageBox.Show($"Only {availableStock} units available in stock for {product.PName}.", "Stock Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                existingItem.Quantity++;
            }
            else
            {
                decimal price = GetSellingPriceByBarcode(barcode);
                if (price <= 0)
                {
                    MessageBox.Show($"No valid stock entry found for product {product.PName}.", "Stock Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                items.Add(new BillItem
                {
                    Product = product,
                    Quantity = 1,
                    Price = price
                });
            }

            return true;
        }
        private int GetAvailableStock(long barcode)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT SUM(s.quantity)
            FROM Stocks s
            INNER JOIN Products p ON p.ID = s.barcode_id
            WHERE p.Barcode = @barcode";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return 0;
        }



        private Product GetProductByBarcode(long barcode)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT Barcode, PName, Category, PStatus
                         FROM Products
                         WHERE Barcode = @barcode";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                PBarcode = Convert.ToInt64(reader["Barcode"]),
                                PName = reader["PName"].ToString(),
                                PCategory = reader["Category"].ToString(),
                                PStatus = reader["PStatus"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
        private decimal GetSellingPriceByBarcode(long barcode)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT TOP 1 s.sp_unit
                 FROM Stocks s
                 INNER JOIN Products p ON p.ID = s.barcode_id
                 WHERE p.Barcode = @barcode AND s.quantity > 0
                 ORDER BY s.expiry_date ASC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDecimal(result);
                    }
                }
            }
            return 0m; // fallback price if not found
        }


        public decimal CalculateSubtotal()
        {
            return items.Sum(i => i.Total);
        }

        public void Clear()
        {
            items.Clear();
        }
        ////////////////// Buttons ////////////////////////////
        public bool SaveCart( decimal cashReceived, decimal changeReturned)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    decimal subtotal = CalculateSubtotal();
                    decimal totalPayable = subtotal;

                    // Insert into Orders table
                    string orderQuery = @"INSERT INTO Orders (Date, Subtotal, TotalPayable, CashReceived, ChangeReturned)
                                  VALUES (@date, @subtotal, @total, @cash, @change);
                                  SELECT SCOPE_IDENTITY();";

                    int orderId;
                    using (SqlCommand cmd = new SqlCommand(orderQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@total", totalPayable);
                        cmd.Parameters.AddWithValue("@cash", cashReceived);
                        cmd.Parameters.AddWithValue("@change", changeReturned);

                        orderId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insert items into OrderDetails table & update stock
                    foreach (var item in Items)
                    {
                        // Insert item into OrderDetails
                        string detailsQuery = @"INSERT INTO OrderDetails (OrderID, Barcode, ProductName, Quantity, UnitPrice, TotalPrice)
                                        VALUES (@orderId, @barcode, @name, @qty, @price, @total)";
                        using (SqlCommand cmd = new SqlCommand(detailsQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@orderId", orderId);
                            cmd.Parameters.AddWithValue("@barcode", item.Product.PBarcode);
                            cmd.Parameters.AddWithValue("@name", item.Product.PName);
                            cmd.Parameters.AddWithValue("@qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@price", item.Price);
                            cmd.Parameters.AddWithValue("@total", item.Total);

                            cmd.ExecuteNonQuery();
                        }

                        // Reduce stock quantity in Stocks table using FIFO (earliest expiry)
                        string stockUpdate = @"
WITH cte AS (
    SELECT TOP (1) *
    FROM Stocks
    WHERE barcode_id = (SELECT ID FROM Products WHERE Barcode = @barcode)
      AND quantity >= @qty
)
UPDATE cte
SET quantity = quantity - @qty
";


                        using (SqlCommand cmd = new SqlCommand(stockUpdate, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@qty", item.Quantity);
                            cmd.Parameters.AddWithValue("@barcode", item.Product.PBarcode);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new InvalidOperationException($"Insufficient stock for product barcode {item.Product.PBarcode}.");
                            }
                        }
                    }

                    transaction.Commit();
                    Clear(); // clear internal list or cart
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show(ex.Message);
                    // Optionally log ex.Message here for debugging
                    return false;
                }
            }
        }

        public void RemoveItem(BillItem item)
        {
            items.Remove(item);
        }




    }
}
