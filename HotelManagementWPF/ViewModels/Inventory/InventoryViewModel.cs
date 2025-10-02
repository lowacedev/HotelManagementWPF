using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;

namespace HotelManagementWPF.ViewModels.Inventory
{
    public class InventoryViewModel
    {
        public int TotalItemsCount { get; set; } = 1200;
        public int OutOfStockCount { get; set; } = 50;
        public int LowStockCount { get; set; } = 100;
        public int OverstockCount { get; set; } = 30;
        public int RecentlyAddedCount { get; set; } = 20;

        public decimal TotalInventoryValue { get; set; } = 90000; // Example

        public int ItemsAddedThisMonth { get; set; } = 200;
        public int ItemsSoldThisMonth { get; set; } = 180;
        public int DamagedItemsCount { get; set; } = 5;

        public ChartValues<int> ItemsAddedValues { get; set; }
        public ChartValues<int> ItemsUsedValues { get; set; }

        public string[] Months { get; set; } = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public SeriesCollection CategoryDistributionSeries { get; set; }

        public InventoryViewModel()
        {
            // Sample data for charts
            ItemsAddedValues = new ChartValues<int> { 20, 18, 22, 19, 25, 30, 28, 35, 40, 38, 42, 45 };
            ItemsUsedValues = new ChartValues<int>{ 15, 17, 20, 18, 22, 25, 23, 30, 28};

            // Sample pie chart data
            CategoryDistributionSeries = new SeriesCollection
            {
                new PieSeries { Title="Electronics", Values=new ChartValues<decimal> { 40 }, DataLabels=true },
                new PieSeries { Title="Furniture", Values=new ChartValues<decimal> { 25 }, DataLabels=true },
                new PieSeries { Title="Clothing", Values=new ChartValues<decimal> { 15 }, DataLabels=true },
                new PieSeries { Title="Kitchen", Values=new ChartValues<decimal> { 20 }, DataLabels=true },
            };
        }
    }
}