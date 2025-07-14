using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Sync
{
    internal class Chart
    {
        public Dictionary<DateTime, decimal> GetDailySales()
        {
            var sales = new Dictionary<DateTime, decimal>();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT CAST(Date AS DATE) AS SaleDate, SUM(TotalPayable) AS TotalSales
                FROM Orders
                GROUP BY CAST(Date AS DATE)
                ORDER BY SaleDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime date = Convert.ToDateTime(reader["SaleDate"]);
                        decimal total = Convert.ToDecimal(reader["TotalSales"]);
                        sales[date] = total;
                    }
                }
            }

            return sales;
        }

        /////////////////////////////////////////////////////////////////////

        public Dictionary<string, int> GetTop7SoldProducts()
        {
            var topProducts = new Dictionary<string, int>();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT TOP 7 ProductName, SUM(Quantity) AS TotalSold
            FROM OrderDetails
            GROUP BY ProductName
            ORDER BY TotalSold DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string productName = reader["ProductName"].ToString();
                        int totalSold = Convert.ToInt32(reader["TotalSold"]);
                        topProducts[productName] = totalSold;
                    }
                }
            }

            return topProducts;
        }

        /////////////////////////////////////////////////////////////////////

        public Dictionary<string, decimal> GetTopGrossingProducts()
        {
            Dictionary<string, decimal> productRevenue = new Dictionary<string, decimal>();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT TOP 7 ProductName, SUM(UnitPrice * Quantity) AS TotalRevenue
                             FROM OrderDetails
                             GROUP BY ProductName
                             ORDER BY TotalRevenue DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader["ProductName"].ToString();
                        decimal revenue = Convert.ToDecimal(reader["TotalRevenue"]);
                        productRevenue[name] = revenue;
                    }
                }

                return productRevenue;
            }
        }
    }
}

