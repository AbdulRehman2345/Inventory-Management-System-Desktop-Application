using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Sync
{
    internal static  class Buttons 
    {
        public static void OpenStockForm(Form currentForm)
        {
            StockForm stockForm = new StockForm();
            stockForm.FormClosed += (s, args) => currentForm.Close();

            stockForm.Show();
            currentForm.Hide();
        }
        public static void OpenProductForm(Form currentForm)
        {
            ProductsForm productsForm = new ProductsForm();
            productsForm.FormClosed += (s, args) => currentForm.Close();

            productsForm.Show();
            currentForm.Hide();
        }
        public static void OpenBillForm(Form currentForm)
        {
            BillForm BillForm = new BillForm();
            BillForm.FormClosed += (s, args) => currentForm.Close();

            BillForm.Show();
            currentForm.Hide();
        }
        public static void OpenChartForm(Form currentForm)
        {
            ChartsFormcs ChartForm = new ChartsFormcs();
            ChartForm.FormClosed += (s, args) => currentForm.Close();

            ChartForm.Show();
            currentForm.Hide();
        }
        public static void OpenReportForm(Form currentForm)
        {
            ReportForm ReportForm = new ReportForm();
            ReportForm.FormClosed += (s, args) => currentForm.Close();

            ReportForm.Show();
            currentForm.Hide();
        }
        public static void OpenNotificationForm(Form currentForm)
        {
            NotificationForm NotificationForm = new NotificationForm();
            NotificationForm.FormClosed += (s, args) => currentForm.Close();

            NotificationForm.Show();
            currentForm.Hide();
        }
        public static void OpenLoginForm(Form currentForm)
        {
            LoginForm LoginForm = new LoginForm();
            LoginForm.FormClosed += (s, args) => currentForm.Close();

            LoginForm.Show();
            currentForm.Hide();
        }
    }
}
