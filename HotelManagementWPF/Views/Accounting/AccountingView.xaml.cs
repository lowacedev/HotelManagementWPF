using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.Views.Accounting
{
    public partial class AccountingView : UserControl
    {
        public AccountingView()
        {
            InitializeComponent();
            LoadStaticData();

            // Initialize placeholder text
            InitializePlaceholder(MainSearchBox);
        }

        private void LoadStaticData()
        {
            // Sample data for Total Sales
            var totalSalesData = new List<TotalSale>
            {
                new TotalSale { Date = "2023-10-01", Item = "Room Booking", Amount = "$5000" },
                new TotalSale { Date = "2023-10-02", Item = "Food & Beverage", Amount = "$2000" },
                new TotalSale { Date = "2023-10-03", Item = "Spa Services", Amount = "$1500" }
            };
            TotalSalesDataGrid.ItemsSource = totalSalesData;

            // Sample data for Inventory Expenses
            var inventoryExpensesData = new List<InventoryExpense>
            {
                new InventoryExpense { Item = "Soap", Quantity = 50, Cost = "$100" },
                new InventoryExpense { Item = "Towels", Quantity = 20, Cost = "$200" },
                new InventoryExpense { Item = "Bedding", Quantity = 30, Cost = "$300" }
            };
            InventoryExpensesDataGrid.ItemsSource = inventoryExpensesData;

            // Sample data for Labor Expenses
            var laborExpensesData = new List<LaborExpense>
            {
                new LaborExpense { Employee = "John Doe", Hours = 40, Cost = "$1000" },
                new LaborExpense { Employee = "Jane Smith", Hours = 35, Cost = "$900" },
                new LaborExpense { Employee = "Bob Johnson", Hours = 45, Cost = "$1100" }
            };
            LaborExpensesDataGrid.ItemsSource = laborExpensesData;
        }

        private void InitializePlaceholder(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Search...";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Text == "Search...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search...";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void ExportAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export all data to PDF");
        }
    }
}