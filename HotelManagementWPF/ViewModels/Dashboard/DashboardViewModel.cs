using HotelManagementWPF;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

public class DashboardViewModel : INotifyPropertyChanged
{
    // Chart data
    public ChartValues<double> LastYearRevenueValues { get; set; }
    public ChartValues<double> ThisYearRevenueValues { get; set; }
    public string[] Months { get; set; }
    public SeriesCollection RoomTypeRevenueSeries { get; set; }

    // For revenue line chart
    private SeriesCollection _revenueSeries;
    public SeriesCollection RevenueSeries
    {
        get => _revenueSeries;
        set { _revenueSeries = value; OnPropertyChanged(nameof(RevenueSeries)); }
    }

    // Dashboard metrics
    private int _todaysCheckInCount;
    public int TodaysCheckInCount
    {
        get => _todaysCheckInCount;
        set { _todaysCheckInCount = value; OnPropertyChanged(nameof(TodaysCheckInCount)); }
    }

    private int _todaysCheckOutCount;
    public int TodaysCheckOutCount
    {
        get => _todaysCheckOutCount;
        set { _todaysCheckOutCount = value; OnPropertyChanged(nameof(TodaysCheckOutCount)); }
    }

    private int _inHotelCount;
    public int InHotelCount
    {
        get => _inHotelCount;
        set { _inHotelCount = value; OnPropertyChanged(nameof(InHotelCount)); }
    }

    private int _availableRoomsCount;
    public int AvailableRoomsCount
    {
        get => _availableRoomsCount;
        set { _availableRoomsCount = value; OnPropertyChanged(nameof(AvailableRoomsCount)); }
    }

    private int _occupiedRoomsCount;
    public int OccupiedRoomsCount
    {
        get => _occupiedRoomsCount;
        set { _occupiedRoomsCount = value; OnPropertyChanged(nameof(OccupiedRoomsCount)); }
    }

    private decimal _totalRevenue;
    public decimal TotalRevenue
    {
        get => _totalRevenue;
        set { _totalRevenue = value; OnPropertyChanged(nameof(TotalRevenue)); }
    }

    private CancellationTokenSource _cts;

    public DashboardViewModel()
    {
        // Initialize months
        Months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        // Sample data for last year
        LastYearRevenueValues = new ChartValues<double> { 4000, 4200, 4100, 5000, 4600, 4220, 4500, 6000, 3500, 8000, 8200, 10000 };

        // Sample data for this year
        var thisYearData = new double?[] { 2500, 1500, 4985, 3156, 6542, 5000, 4620, 4200, 6120 };
        ThisYearRevenueValues = new ChartValues<double>();
        foreach (var val in thisYearData)
            ThisYearRevenueValues.Add(val ?? 0);

        // Setup line chart series
        RevenueSeries = new SeriesCollection
        {
            new LineSeries { Title = "2024", Values = LastYearRevenueValues, Stroke = System.Windows.Media.Brushes.Blue, Fill=System.Windows.Media.Brushes.Transparent, PointGeometrySize=10},
            new LineSeries { Title = "2025", Values = ThisYearRevenueValues, Stroke = System.Windows.Media.Brushes.Green, Fill=System.Windows.Media.Brushes.Transparent, PointGeometrySize=10}
        };

        // Setup pie chart series with custom DataLabelsTemplate
        RoomTypeRevenueSeries = new SeriesCollection
        {
            new PieSeries { Title="Single", Values=new ChartValues<double>{50}, DataLabels=true, LabelPoint = chartPoint => $"{chartPoint.Y}", Tag= "Single" },
            new PieSeries { Title="Double", Values=new ChartValues<double>{30}, DataLabels=true, LabelPoint = chartPoint => $"{chartPoint.Y}", Tag= "Double" },
            new PieSeries { Title="Suite", Values=new ChartValues<double>{20}, DataLabels=true, LabelPoint = chartPoint => $"{chartPoint.Y}", Tag= "Suite" }
        };

        // Start auto-refresh
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        _cts = new CancellationTokenSource();
        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await RefreshDashboardDataAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), _cts.Token);
            }
        }, _cts.Token);
    }

    public void StopAutoRefresh()
    {
        _cts.Cancel();
    }

    public async Task RefreshDashboardDataAsync()
    {
        try
        {
            // Replace with actual database queries
            // For demonstration, using dummy data
            await Task.Delay(100); // simulate async DB call

            // Example: update counts with dummy data
            App.Current.Dispatcher.Invoke(() =>
            {
                TodaysCheckInCount = new Random().Next(0, 20);
                TodaysCheckOutCount = new Random().Next(0, 20);
                InHotelCount = new Random().Next(20, 50);
                AvailableRoomsCount = new Random().Next(10, 30);
                OccupiedRoomsCount = new Random().Next(50, 100);
                TotalRevenue = new decimal(new Random().Next(10000, 50000));
            });
        }
        catch (Exception)
        {
            // handle exceptions
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}