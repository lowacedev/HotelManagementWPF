using DatabaseProject;
using HotelManagementWPF.ViewModels.Accounting;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class DashboardViewModel : INotifyPropertyChanged
{
    // Declare an instance of AccountingViewModel
    private readonly AccountingViewModel _accountingVM;

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

    // Revenue charts
    public ChartValues<double> LastYearRevenueValues { get; set; }
    public ChartValues<double> ThisYearRevenueValues { get; set; }
    public string[] Months { get; private set; }
    public SeriesCollection RoomTypeRevenueSeries { get; private set; }

    public DashboardViewModel()
    {
        // Initialize months
        Months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        LastYearRevenueValues = new ChartValues<double>(new double[12]);
        // Initialize with 10 months for this year (Jan to Oct)
        ThisYearRevenueValues = new ChartValues<double>(new double[10]);
        RoomTypeRevenueSeries = new SeriesCollection();

        // Instantiate your AccountingViewModel
        _accountingVM = new AccountingViewModel();

        // Initialize dashboard data
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        await LoadDashboardMetricsAsync();
        await LoadRevenueDataAsync();
        await LoadRoomTypeRevenueSeriesAsync();
        // After loading all, update TotalRevenue from AccountingViewModel
        UpdateTotalRevenueFromAccounting();
    }

    public async Task LoadDashboardMetricsAsync()
    {
        using (var db = new DbConnections())
        {
            try
            {
                // Check-in count today
                string queryCheckIn = "SELECT COUNT(*) FROM tbl_Booking WHERE CAST(check_in AS DATE) = CAST(GETDATE() AS DATE)";
                var checkInCount = await db.ExecuteScalarAsync(queryCheckIn);
                TodaysCheckInCount = Convert.ToInt32(checkInCount);

                // Check-out count today
                string queryCheckOut = "SELECT COUNT(*) FROM tbl_Booking WHERE CAST(check_out AS DATE) = CAST(GETDATE() AS DATE)";
                var checkOutCount = await db.ExecuteScalarAsync(queryCheckOut);
                TodaysCheckOutCount = Convert.ToInt32(checkOutCount);

                // In hotel
                string queryInHotel = "SELECT COUNT(*) FROM tbl_Booking WHERE GETDATE() BETWEEN check_in AND check_out";
                var inHotelCount = await db.ExecuteScalarAsync(queryInHotel);
                InHotelCount = Convert.ToInt32(inHotelCount);

                // Available rooms
                string queryAvailable = "SELECT COUNT(*) FROM tbl_Room WHERE roomStatus='Available'";
                var available = await db.ExecuteScalarAsync(queryAvailable);
                AvailableRoomsCount = Convert.ToInt32(available);

                // Occupied rooms
                string queryOccupied = "SELECT COUNT(*) FROM tbl_Room WHERE roomStatus='Occupied'";
                var occupied = await db.ExecuteScalarAsync(queryOccupied);
                OccupiedRoomsCount = Convert.ToInt32(occupied);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error loading metrics: {ex.Message}");
            }
        }
    }

    public async Task LoadRevenueDataAsync()
    {
        using (var db = new DbConnections())
        {
            try
            {
                // Last year's revenue per month
                string queryLastYear = @"
                    SELECT MONTH(check_in) AS Month, SUM(totalAmount) AS Revenue
                    FROM tbl_Booking
                    WHERE YEAR(check_in) = YEAR(GETDATE()) - 1
                    GROUP BY MONTH(check_in)";

                var dtLastYear = new DataTable();
                await db.readDatathroughAdapterAsync(queryLastYear, dtLastYear);
                var lastYearValues = new ChartValues<double>(new double[12]);
                foreach (DataRow row in dtLastYear.Rows)
                {
                    int month = Convert.ToInt32(row["Month"]);
                    double revenue = Convert.ToDouble(row["Revenue"]);
                    lastYearValues[month - 1] = revenue;
                }
                LastYearRevenueValues = lastYearValues;

                // This year's revenue per month (only Jan to Oct)
                string queryThisYear = @"
                    SELECT MONTH(check_in) AS Month, SUM(totalAmount) AS Revenue
                    FROM tbl_Booking
                    WHERE YEAR(check_in) = YEAR(GETDATE())
                    GROUP BY MONTH(check_in)";

                var dtThisYear = new DataTable();
                await db.readDatathroughAdapterAsync(queryThisYear, dtThisYear);

                var thisYearValues = new ChartValues<double>(new double[10]);
                foreach (DataRow row in dtThisYear.Rows)
                {
                    int month = Convert.ToInt32(row["Month"]);
                    double revenue = Convert.ToDouble(row["Revenue"]);
                    // Populate only months Jan (1) to Oct (10)
                    if (month >= 1 && month <= 10)
                    {
                        thisYearValues[month - 1] = revenue;
                    }
                }
                ThisYearRevenueValues = thisYearValues;

                // Notify UI
                OnPropertyChanged(nameof(LastYearRevenueValues));
                OnPropertyChanged(nameof(ThisYearRevenueValues));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading revenue data: {ex.Message}");
            }
        }
    }

    public async Task LoadRoomTypeRevenueSeriesAsync()
    {
        using (var db = new DbConnections())
        {
            try
            {
                // Corrected query with JOIN
                string query = @"
                SELECT r.roomType, SUM(b.totalAmount) AS Revenue
                FROM tbl_Booking b
                JOIN tbl_Room r ON b.room_id = r.room_id
                GROUP BY r.roomType";

                var dt = new DataTable();
                await db.readDatathroughAdapterAsync(query, dt);

                var series = new SeriesCollection();

                foreach (DataRow row in dt.Rows)
                {
                    string type = row["roomType"].ToString();
                    double revenue = Convert.ToDouble(row["Revenue"]);
                    series.Add(new PieSeries
                    {
                        Title = type,
                        Values = new ChartValues<double> { revenue },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"₱{chartPoint.Y:N2}"
                    });
                }

                RoomTypeRevenueSeries = series;
                OnPropertyChanged(nameof(RoomTypeRevenueSeries));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading room type revenue series: {ex.Message}");
            }
        }
    }

    // Method to update TotalRevenue from AccountingViewModel
    public void UpdateTotalRevenueFromAccounting()
    {
        // Ensure the AccountingViewModel has finished loading and calculating
        TotalRevenue = _accountingVM.TotalRevenue;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}