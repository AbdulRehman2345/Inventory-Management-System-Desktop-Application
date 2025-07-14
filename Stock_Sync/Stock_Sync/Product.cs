using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Stock_Sync
{
    internal class Product : IDataOperations
    {
        private long pBarcode;
        private String pName;
        private String pCategory;
        private String pStatus;

        // Start getter setter
        public long PBarcode
        {
            get { return pBarcode; }
            set { pBarcode = value; }
        }

        public string PName
        {
            get { return pName; }
            set { pName = value; }
        }

        public string PCategory
        {
            get { return pCategory; }
            set { pCategory = value; }
        }

        public String PStatus
        {
            get { return pStatus; }
            set { pStatus = value; }
        }

        // End getter setter

        // Start Add Method
        public bool Add()
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Products (Barcode, PName, Category,PStatus) VALUES (@Barcode, @Name, @Category,@Status)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Barcode", pBarcode);
                        cmd.Parameters.AddWithValue("@Name", pName);
                        cmd.Parameters.AddWithValue("@Category", pCategory);
                        cmd.Parameters.AddWithValue("@Status", pStatus);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
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
                    string query = @"
                    SELECT 
                        ID, 
                        Barcode, 
                        PName AS [Product Name], 
                        Category AS [Product Category], 
                        PStatus AS [Product Status], 
                        DateAdded AS [Date Added]
                    FROM Products";

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
                    string query = "SELECT * FROM Products WHERE ID = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PBarcode = Convert.ToInt64(reader["Barcode"]);
                                PName = reader["PName"].ToString();
                                PCategory = reader["Category"].ToString();
                                PStatus = reader["PStatus"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Product not found.");
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
                    string query = "UPDATE Products SET Barcode = @Barcode, PName = @Name, Category = @Category, PStatus = @Status WHERE ID = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Barcode", pBarcode);
                        cmd.Parameters.AddWithValue("@Name", pName);
                        cmd.Parameters.AddWithValue("@Category", pCategory);
                        cmd.Parameters.AddWithValue("@Status", pStatus);
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
                    string query = "DELETE FROM Products WHERE ID = @Id";
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
        public static DataTable SearchProducts(string keyword)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    ID, 
                    Barcode, 
                    PName AS [Product Name], 
                    Category AS [Product Category], 
                    PStatus AS [Product Status], 
                    DateAdded AS [Date Added]
                FROM Products
                WHERE PName LIKE @keyword 
                   OR CAST(Barcode AS NVARCHAR) LIKE @keyword";

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
                MessageBox.Show("Error during product search: " + ex.Message);
            }

            return dt;
        }
        // End Search method

    }
}

