using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String Email = email.Text.Trim();
            String Password = password.Text.Trim();

            var user = Login.Authenticate(Email, Password);

            if (user != null && user.Role == "Staff")
            {
                Session.Email = user.Email;
                Session.Role = user.Role;
                ProductsForm product = new ProductsForm();
                this.Hide();
                product.Show();
            }
            else
            {
                MessageBox.Show("Access denied! Not a Staff account.");
            }
            //ProductsForm productsForm = new ProductsForm();
            //productsForm.FormClosed += (s, args) => this.Close();
            //productsForm.Show();
            //this.Hide();

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            String Email = email.Text.Trim();
            String Password = password.Text.Trim();

            var user = Login.Authenticate(Email, Password);

            if (user != null && user.Role == "Admin")
            {
                Session.Email = user.Email;
                Session.Role = user.Role;
                ProductsForm product = new ProductsForm();
                this.Hide();
                product.Show();
            }
            else
            {
                MessageBox.Show("Access denied! Not an Admin account.");
            }
        }
    }
}
