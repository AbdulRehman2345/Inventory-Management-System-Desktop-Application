using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Collections;

namespace Stock_Sync
{
    internal class Stock
    {
        private int barcode_id;
        private String stock_code;
        private int quantity;
        private int minimum_stock;
        private double cp_uint;
        private double sp_unit;
        private DateTime arrival_date;
        private DateTime? expiry_date;

        // Start getter setter
        public int Barcode_id
        {
            get { return barcode_id; }
            set { barcode_id = value; }
        }

        public String Stock_code
        {
            get { return stock_code; }
            set { stock_code = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public int Minimum_stock
        {
            get { return minimum_stock; }
            set { minimum_stock = value; }
        }

        public double Cp_unit
        {
            get { return cp_uint; }
            set { cp_uint = value; }
        }
        public double Sp_unit
        {
            get { return sp_unit; }
            set { sp_unit = value; }
        }
        public DateTime Arrival_date
        {
            get { return arrival_date; }
            set { arrival_date = value; }
        }

        public DateTime? Expirey_Date
        {
            get { return expiry_date; }
            set { expiry_date = value; }
        }

        // end getter setter

        // Start Add Method
        public bool Add()
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Stocks (barcode_id, stock_code, quantity, minimum_stock, cp_unit, sp_unit, arrival_date, expiry_date) " +
                                   "VALUES (@barcode_id, @stock_code, @quantity, @minimum_stock, @cp_unit, @sp_unit, @arrival_date, @ExpiryDate)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@barcode_id", barcode_id);
                        cmd.Parameters.AddWithValue("@stock_code", stock_code);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@minimum_stock", minimum_stock);
                        cmd.Parameters.AddWithValue("@cp_unit", cp_uint);
                        cmd.Parameters.AddWithValue("@sp_unit", sp_unit);
                        cmd.Parameters.AddWithValue("@arrival_date", arrival_date);

                        if (expiry_date.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ExpiryDate", expiry_date.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ExpiryDate", DBNull.Value);
                        }

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        // End Add Method

        // Start View Method
        public static DataTable ViewAll()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                    s.stock_id AS [Stock Id], 
                    p.Barcode, 
                    p.PName AS [Product Name], 
                    s.stock_code AS [Stock Code], 
                    s.quantity AS Quantity, 
                    s.cp_unit AS [Cost Price/Unit], 
                    s.sp_unit AS [Selling Price/Unit], 
                    s.expiry_date AS [Expiry Date],
                    s.date_added AS [Date Added]
                    FROM 
                    Stocks s
                    INNER JOIN 
                    Products p ON s.barcode_id = p.ID
                     ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products");
            }

            return dt;
        }

        //  End View Method

        // Start Data to textboxes method
        public void FetchDetailId(int Id)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Stocks WHERE stock_id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                barcode_id = Convert.ToInt32(reader["barcode_id"]);
                                stock_code = reader["stock_code"].ToString();
                                quantity = Convert.ToInt32(reader["quantity"]);
                                minimum_stock = Convert.ToInt32(reader["minimum_stock"]);
                                cp_uint = Convert.ToDouble(reader["cp_unit"]);
                                sp_unit = Convert.ToDouble(reader["sp_unit"]);
                                arrival_date = Convert.ToDateTime(reader["arrival_date"]);
                                if (reader["expiry_date"] != DBNull.Value)
                                {
                                    expiry_date = Convert.ToDateTime(reader["expiry_date"]);
                                }
                                else
                                {
                                    expiry_date = null;
                                }

                            }
                            else
                            {
                                MessageBox.Show("Stocks not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error !");
            }
        }
        // End Data to textboxes method

        // Start Update Method
        public bool Update(int id)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Stocks 
                             SET 
                                 barcode_id = @BarcodeId,
                                 stock_code = @StockCode,
                                 quantity = @Quantity,
                                 minimum_stock = @MinimumStock,
                                 cp_unit = @CPUnit,
                                 sp_unit = @SPUnit,
                                 arrival_date = @ArrivalDate,
                                 expiry_date = @ExpiryDate
                             WHERE stock_id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BarcodeId", barcode_id);
                        cmd.Parameters.AddWithValue("@StockCode", stock_code);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@MinimumStock", minimum_stock);
                        cmd.Parameters.AddWithValue("@CPUnit", cp_uint);
                        cmd.Parameters.AddWithValue("@SPUnit", sp_unit);
                        cmd.Parameters.AddWithValue("@ArrivalDate", arrival_date);

                        if (expiry_date.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ExpiryDate", expiry_date.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ExpiryDate", DBNull.Value);
                        }

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // End Update Method

        // Start Delete Method
        public bool Delete(int id)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Stocks WHERE stock_id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // End Delete Method

        // Start Search method
        public static DataTable SearchStocks(string keyword)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                   s.stock_id AS [Stock Id], 
                    p.Barcode, 
                    p.PName AS [Product Name], 
                    s.stock_code AS [Stock Code], 
                    s.quantity AS Quantity, 
                    s.cp_unit AS [Cost Price/Unit], 
                    s.sp_unit AS [Selling Price/Unit], 
                    s.expiry_date AS [Expiry Date],
                    s.date_added AS [Date Added]
                FROM Stock s
                INNER JOIN Products p ON s.barcode_id = p.Barcode
                WHERE 
                    p.PName LIKE @keyword
                    OR CAST(p.Barcode AS NVARCHAR) LIKE @keyword";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during stock search: " + ex.Message);
            }

            return dt;
        }

        // End Search method
    }
}
