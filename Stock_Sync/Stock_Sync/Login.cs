using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Sync
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public static Login Authenticate(string email, string password)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT Email, Password, Role FROM Users WHERE Email = @Email AND Password = @Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Login
                    {
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }

                return null;
            }
        }
    }

}
