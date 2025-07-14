using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Stock_Sync
{
    public partial class ChartsFormcs : Form
    {
        public ChartsFormcs()
        {
            InitializeComponent();
        }

        private void ChartsFormcs_Load(object sender, EventArgs e)
        {
            LoadDailySalesChart();
            LoadTop7SoldProductsPieChart();
            LoadTopGrossingProductsChart();
        }
        Chart report = new Chart();
        private void LoadDailySalesChart()
        {

            var dailySales = report.GetDailySales();

            chart1.Series.Clear();

            var series = chart1.Series.Add("Daily Sales");
             series.ChartType = SeriesChartType.StackedColumn;
            series.IsValueShownAsLabel = true;

            foreach (var entry in dailySales)
            {
                series.Points.AddXY(entry.Key.ToString("MM-dd"), entry.Value);
            }

            chart1.ChartAreas[0].AxisX.Title = "Date";
            chart1.ChartAreas[0].AxisY.Title = "Total Sales";
            chart1.ChartAreas[0].RecalculateAxesScale();
        }
        private void LoadTop7SoldProductsPieChart()
        {
            chart2.Series.Clear();  // Assume chart2 is your second chart control

            var data = report.GetTop7SoldProducts();

            var series = chart2.Series.Add("Top 7 Sold Products");
            series.ChartType = SeriesChartType.Line;
            series.IsValueShownAsLabel = true;

            foreach (var entry in data)
            {
                series.Points.AddXY(entry.Key, entry.Value);
            }

            chart2.Titles.Clear();
        }
        private void LoadTopGrossingProductsChart()
        {
            var data = report.GetTopGrossingProducts();

            chart3.Series.Clear();
            chart3.Titles.Clear();

            Series series = new Series("Revenue");
            series.ChartType = SeriesChartType.Bar;
            series.IsValueShownAsLabel = true;

            foreach (var entry in data)
            {
                series.Points.AddXY(entry.Key, entry.Value);
            }
            chart3.Series.Add(series);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        ////// Buttons ///////
        private void button1_Click(object sender, EventArgs e)
        {
            Buttons.OpenProductForm(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Buttons.OpenStockForm(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Buttons.OpenBillForm(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Buttons.OpenNotificationForm(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Buttons.OpenReportForm(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Buttons.OpenLoginForm(this);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}

    

