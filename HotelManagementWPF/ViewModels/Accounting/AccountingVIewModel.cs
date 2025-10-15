using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseProject;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.ViewModels.Accounting
{
    public class AccountingViewModel : INotifyPropertyChanged
    {
        private readonly DbConnections _db;

        // All data collections (unfiltered)
        private ObservableCollection<TotalSaleDisplay> _allTotalSales;
        private ObservableCollection<InventoryExpenseDisplay> _allInventoryExpenses;
        private ObservableCollection<LaborExpenseDisplay> _allLaborExpenses;

        // Displayed data collections (filtered)
        public ObservableCollection<TotalSaleDisplay> TotalSales { get; set; }
        public ObservableCollection<InventoryExpenseDisplay> InventoryExpenses { get; set; }
        public ObservableCollection<LaborExpenseDisplay> LaborExpenses { get; set; }

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(nameof(TotalRevenue)); }
        }

        private decimal _totalSalesAmount;
        public decimal TotalSalesAmount
        {
            get => _totalSalesAmount;
            set { _totalSalesAmount = value; OnPropertyChanged(nameof(TotalSalesAmount)); }
        }

        private decimal _totalInventoryExpenses;
        public decimal TotalInventoryExpenses
        {
            get => _totalInventoryExpenses;
            set { _totalInventoryExpenses = value; OnPropertyChanged(nameof(TotalInventoryExpenses)); }
        }

        private decimal _totalLaborExpenses;
        public decimal TotalLaborExpenses
        {
            get => _totalLaborExpenses;
            set { _totalLaborExpenses = value; OnPropertyChanged(nameof(TotalLaborExpenses)); }
        }

        public DateTime? FilterMonth { get; set; } // for filtering

        public ICommand LoadDataCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AccountingViewModel()
        {
            _db = new DbConnections();

            TotalSales = new ObservableCollection<TotalSaleDisplay>();
            InventoryExpenses = new ObservableCollection<InventoryExpenseDisplay>();
            LaborExpenses = new ObservableCollection<LaborExpenseDisplay>();

            _allTotalSales = new ObservableCollection<TotalSaleDisplay>();
            _allInventoryExpenses = new ObservableCollection<InventoryExpenseDisplay>();
            _allLaborExpenses = new ObservableCollection<LaborExpenseDisplay>();

            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadTotalSalesAsync();
            await LoadInventoryExpensesAsync();
            await LoadLaborExpensesAsync();
            ApplyFilter();
        }

        private async Task LoadTotalSalesAsync()
        {
            _allTotalSales.Clear();
            var dt = await _db.readDataWithParametersAsync(@"
        SELECT 
            g.name AS GuestName, 
            r.roomNumber AS RoomNumber, 
            r.roomType AS RoomType, 
            b.totalPaid, 
            b.datecreated
        FROM tbl_Guest g
        JOIN tbl_Booking b ON g.guest_id = b.guest_id
        JOIN tbl_Room r ON b.room_id = r.room_id
        WHERE b.check_out >= DATEADD(day, -30, GETDATE())", null);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                var dateStr = row["datecreated"].ToString();
                DateTime date = DateTime.Parse(dateStr);
                var display = new TotalSaleDisplay
                {
                    GuestName = row["GuestName"].ToString(),
                    RoomNumber = row["RoomNumber"].ToString(),
                    RoomType = row["RoomType"].ToString(),
                    TotalPaid = $"₱{Convert.ToDecimal(row["totalPaid"]):N2}",
                    DateCreated = date.ToString("yyyy-MM-dd"),
                    DateObject = date
                };
                _allTotalSales.Add(display);
            }
            ApplyFilter();
        }

        private async Task LoadInventoryExpensesAsync()
        {
            _allInventoryExpenses.Clear();
            var dt = await _db.readDataWithParametersAsync(
                "SELECT itemName, quantity, price, restockQuantity FROM tbl_Inventory_item WHERE restockQuantity != 0", null);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                decimal price = Convert.ToDecimal(row["price"]);
                int qty = Convert.ToInt32(row["quantity"]);
                int restockQty = Convert.ToInt32(row["restockQuantity"]);
                decimal totalPrice = price * restockQty;
                var display = new InventoryExpenseDisplay
                {
                    ItemName = row["itemName"].ToString(),
                    Quantity = qty,
                    PricePerItem = $"₱{price:N2}",
                    TotalPrice = $"₱{totalPrice:N2}",
                    Restock = restockQty.ToString(),
                    TotalPriceValue = totalPrice,
                    RestockDate = DateTime.Now // placeholder, replace if actual date
                };
                _allInventoryExpenses.Add(display);
            }
            ApplyFilter();
        }

        private async Task LoadLaborExpensesAsync()
        {
            _allLaborExpenses.Clear();
            var dt = await _db.readDataWithParametersAsync(
                @"SELECT s.name AS StaffName, s.department, p.duty_hours, p.rate, p.total_gross_pay, p.deductions, p.total_net_pay, p.created_at
          FROM tbl_Payroll p
          JOIN tbl_Staff s ON p.staff_id = s.staff_id", null);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                decimal rate = Convert.ToDecimal(row["rate"]);
                decimal gross = Convert.ToDecimal(row["total_gross_pay"]);
                decimal deductions = Convert.ToDecimal(row["deductions"]);
                decimal net = Convert.ToDecimal(row["total_net_pay"]);
                DateTime createdAt = Convert.ToDateTime(row["created_at"]);
                var display = new LaborExpenseDisplay
                {
                    StaffName = row["StaffName"].ToString(),
                    Department = row["department"].ToString(),
                    DutyHours = Convert.ToInt32(row["duty_hours"]),
                    Rate = $"₱{rate:N2}",
                    Gross = $"₱{gross:N2}",
                    Deductions = $"₱{deductions:N2}",
                    NetPay = $"₱{net:N2}",
                    CreatedAt = createdAt.ToString("yyyy-MM-dd"),
                    CreatedAtObject = createdAt
                };
                _allLaborExpenses.Add(display);
            }
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            if (FilterMonth.HasValue)
            {
                var month = FilterMonth.Value;

                // Filter TotalSales
                var filteredSales = new ObservableCollection<TotalSaleDisplay>();
                foreach (var sale in _allTotalSales)
                {
                    if (sale.DateObject.Month == month.Month && sale.DateObject.Year == month.Year)
                        filteredSales.Add(sale);
                }
                TotalSales.Clear();
                foreach (var item in filteredSales)
                    TotalSales.Add(item);

                // Filter InventoryExpenses
                var filteredInventory = new ObservableCollection<InventoryExpenseDisplay>();
                foreach (var inv in _allInventoryExpenses)
                {
                    if (inv.RestockDate.Month == month.Month && inv.RestockDate.Year == month.Year)
                        filteredInventory.Add(inv);
                }
                InventoryExpenses.Clear();
                foreach (var item in filteredInventory)
                    InventoryExpenses.Add(item);

                // Filter LaborExpenses
                var filteredLabor = new ObservableCollection<LaborExpenseDisplay>();
                foreach (var labor in _allLaborExpenses)
                {
                    if (labor.CreatedAtObject.Month == month.Month && labor.CreatedAtObject.Year == month.Year)
                        filteredLabor.Add(labor);
                }
                LaborExpenses.Clear();
                foreach (var item in filteredLabor)
                    LaborExpenses.Add(item);
            }
            else
            {
                // No filter, show all
                TotalSales.Clear();
                foreach (var item in _allTotalSales)
                    TotalSales.Add(item);

                InventoryExpenses.Clear();
                foreach (var item in _allInventoryExpenses)
                    InventoryExpenses.Add(item);

                LaborExpenses.Clear();
                foreach (var item in _allLaborExpenses)
                    LaborExpenses.Add(item);
            }
            // Recalculate totals
            CalculateTotals();
        }

        public void CalculateTotals()
        {
            decimal salesSum = 0;
            foreach (var sale in TotalSales)
            {
                if (decimal.TryParse(sale.TotalPaid.Replace("₱", "").Trim(), out decimal amt))
                    salesSum += amt;
            }
            TotalSalesAmount = salesSum;

            decimal inventorySum = 0;
            foreach (var expense in InventoryExpenses)
            {
                inventorySum += expense.TotalPriceValue;
            }
            TotalInventoryExpenses = inventorySum;

            decimal laborSum = 0;
            foreach (var labor in LaborExpenses)
            {
                if (decimal.TryParse(labor.NetPay.Replace("₱", "").Trim(), out decimal amt))
                    laborSum += amt;
            }
            TotalLaborExpenses = laborSum;

            TotalRevenue = TotalSalesAmount - TotalInventoryExpenses - TotalLaborExpenses;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // RelayCommand Implementation
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public async void Execute(object parameter) => await _execute();

        public event EventHandler CanExecuteChanged;
    }
}