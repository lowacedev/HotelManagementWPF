using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels.Payroll
{
    // RelayCommand implementation for MVVM command binding
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }

    public class PayrollViewModel : BaseViewModel
    {
        public ObservableCollection<PayrollDisplayModel> PaginatedUsers { get; set; } = new ObservableCollection<PayrollDisplayModel>();

        public string SearchText { get; set; } = "";

        // Your database connection class
        private DbConnections db;

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand AddPayrollCommand { get; }
        public ICommand EditPayrollCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        public ObservableCollection<int> PageNumbers { get; } = new ObservableCollection<int>();
        private int currentPage = 1;
        private int totalPages = 1;
        private int pageSize = 10;

        public PayrollViewModel()
        {
            db = new DbConnections();

            // Initialize commands
            AddPayrollCommand = new RelayCommand(OpenAddPayrollForm);
            // You can initialize other commands similarly if needed

            // Load data initially
            LoadPayrollData();
        }

        private void OpenAddPayrollForm()
        {
            // Make sure the namespace matches the location of AddPayrollForm.xaml
            var addPayrollWindow = new HotelManagementWPF.Views.Accounting.AddPayrollForm();
            addPayrollWindow.ShowDialog();

            // Refresh data after closing the form, if needed
            LoadPayrollData();
        }

        public void LoadPayrollData()
        {
            try
            {
                // Create a DataTable to hold the query results
                DataTable dt = new DataTable();

                // Your SQL query
                string query = @"
                    SELECT p.payroll_id, s.name AS StaffName, s.department, p.duty_hours, p.rate, 
                           p.total_gross_pay, p.deductions, p.total_net_pay, p.created_at
                    FROM tbl_Payroll p
                    JOIN tbl_Staff s ON p.staff_id = s.staff_id";

                // Use your DbConnections to fill the DataTable
                db.readDatathroughAdapter(query, dt);

                // Clear existing collection
                PaginatedUsers.Clear();

                // Loop through DataTable rows and populate the ObservableCollection
                foreach (DataRow row in dt.Rows)
                {
                    var payrollItem = new PayrollDisplayModel
                    {
                        PayrollId = Convert.ToInt32(row["payroll_id"]),
                        StaffName = row["StaffName"].ToString(),
                        Department = row["department"].ToString(),
                        DutyHours = Convert.ToInt32(row["duty_hours"]),
                        Rate = Convert.ToDecimal(row["rate"]),
                        TotalGrossPay = Convert.ToDecimal(row["total_gross_pay"]),
                        Deductions = Convert.ToDecimal(row["deductions"]),
                        TotalNetPay = Convert.ToDecimal(row["total_net_pay"]),
                        CreatedAt = Convert.ToString(row["created_at"])
                    };

                    PaginatedUsers.Add(payrollItem);
                }

                // Optionally, implement pagination logic here
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error loading payroll data: " + ex.Message);
            }
        }
    }
}