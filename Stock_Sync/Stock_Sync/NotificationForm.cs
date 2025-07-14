using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    public partial class NotificationForm : Form
    {
        public NotificationForm()
        {
            InitializeComponent();
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            Notification.ShowLowStockAlerts(richTextBox1);
            Notification.ShowExpiredProductAlerts(richTextBox1);
            Notification.ShowNeverSoldItems(richTextBox1);
        }
        ////// Buttons ///////

        private void button3_Click(object sender, EventArgs e)
        {
            Buttons.OpenBillForm(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Buttons.OpenProductForm(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Buttons.OpenStockForm(this);
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
    }
}
