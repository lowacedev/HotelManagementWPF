using DatabaseProject;
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

    // Real-time data
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
        // Initialize chart data with sample values or from database
        LastYearRevenueValues = new ChartValues<double> { 3000, 3500, 4000, 3800, 4200, 4500 };
        ThisYearRevenueValues = new ChartValues<double> { 3200, 3700, 4100, 3900, 4300, 4700 };
        Months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        RoomTypeRevenueSeries = new SeriesCollection
        {
            new PieSeries { Title = "Single", Values = new ChartValues<double> { 40 }, DataLabels = true },
            new PieSeries { Title = "Double", Values = new ChartValues<double> { 35 }, DataLabels = true },
            new PieSeries { Title = "Presidential", Values = new ChartValues<double> { 25 }, DataLabels = true }
        };

        // Start periodic refresh
        StartAutoRefresh();

        // Initial data load
        _ = RefreshDashboardDataAsync();
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
            // Your actual queries here
            string checkInCountQuery = "SELECT COUNT(*) FROM tbl_Booking WHERE check_in = CAST(GETDATE() AS DATE)";
            string checkOutCountQuery = "SELECT COUNT(*) FROM tbl_Booking WHERE check_out = CAST(GETDATE() AS DATE)";
            string inHotelCountQuery = "SELECT COUNT(*) FROM tbl_Booking WHERE check_in <= CAST(GETDATE() AS DATE) AND check_out >= CAST(GETDATE() AS DATE)";
            string availableRoomsQuery = "SELECT COUNT(*) FROM tbl_Room WHERE roomStatus = 'Available'";
            string occupiedRoomsQuery = "SELECT COUNT(*) FROM tbl_Room WHERE roomStatus = 'Booked'";
            string revenueQuery = "SELECT SUM(totalAmount) FROM tbl_Booking WHERE check_in >= DATEADD(day, -30, GETDATE())";

            using (var db = new DbConnections())
            {
                var checkInDtTask = db.readDataWithParametersAsync(checkInCountQuery, null);
                var checkOutDtTask = db.readDataWithParametersAsync(checkOutCountQuery, null);
                var inHotelDtTask = db.readDataWithParametersAsync(inHotelCountQuery, null);
                var availableRoomsDtTask = db.readDataWithParametersAsync(availableRoomsQuery, null);
                var occupiedRoomsDtTask = db.readDataWithParametersAsync(occupiedRoomsQuery, null);
                var revenueDtTask = db.readDataWithParametersAsync(revenueQuery, null);

                await Task.WhenAll(checkInDtTask, checkOutDtTask, inHotelDtTask, availableRoomsDtTask, occupiedRoomsDtTask, revenueDtTask);

                var checkInDt = await checkInDtTask;
                var checkOutDt = await checkOutDtTask;
                var inHotelDt = await inHotelDtTask;
                var availableRoomsDt = await availableRoomsDtTask;
                var occupiedRoomsDt = await occupiedRoomsDtTask;
                var revenueDt = await revenueDtTask;

                // Update properties on UI thread
                App.Current.Dispatcher.Invoke(() =>
                {
                    TodaysCheckInCount = checkInDt.Rows.Count > 0 ? Convert.ToInt32(checkInDt.Rows[0][0]) : 0;
                    TodaysCheckOutCount = checkOutDt.Rows.Count > 0 ? Convert.ToInt32(checkOutDt.Rows[0][0]) : 0;
                    InHotelCount = inHotelDt.Rows.Count > 0 ? Convert.ToInt32(inHotelDt.Rows[0][0]) : 0;
                    AvailableRoomsCount = availableRoomsDt.Rows.Count > 0 ? Convert.ToInt32(availableRoomsDt.Rows[0][0]) : 0;
                    OccupiedRoomsCount = occupiedRoomsDt.Rows.Count > 0 ? Convert.ToInt32(occupiedRoomsDt.Rows[0][0]) : 0;
                    TotalRevenue = revenueDt.Rows.Count > 0 && revenueDt.Rows[0][0] != DBNull.Value ? Convert.ToDecimal(revenueDt.Rows[0][0]) : 0;
                });
            }
        }
        catch (Exception ex)
        {
            // handle errors gracefully
            // e.g. log error
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}