using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    internal class Report
    {
       static DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        ///////////////////////////////////////////////
        public static DataTable ViewOrdersByDate()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    OrderID,
                    Date,
                    TotalPayable AS Total ,
                    CashReceived ,
                    ChangeReturned 
                FROM Orders
                WHERE Date = @Date";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", date.Date);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Error loading orders for report!");
            }

            return dt;
        }

    }
}

