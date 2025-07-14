using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Stock_Sync
{
    public static class DBHelper
    {
        private static readonly string connectionString = "Data Source=ABDULREHMAN\\SQLEXPRESS;Initial Catalog=InventoryDB;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

       
    }
}
