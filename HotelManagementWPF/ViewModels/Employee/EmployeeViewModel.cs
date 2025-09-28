using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Models;
using HotelManagementWPF.Views.Employees;
using DatabaseProject;

namespace HotelManagementWPF.ViewModels.Employee
{
    internal class EmployeeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EmployeeModel> _employees;
        private ObservableCollection<EmployeeModel> _filteredEmployees;
        private ObservableCollection<EmployeeModel> _paginatedEmployees;
        private string _searchText;
        private int _currentPage = 1;
        private const int ItemsPerPage = 10;

        public EmployeeViewModel()
        {
            Employees = new ObservableCollection<EmployeeModel>();
            FilteredEmployees = new ObservableCollection<EmployeeModel>();
            PaginatedEmployees = new ObservableCollection<EmployeeModel>();

            // Initialize commands
            AddEmployeeCommand = new RelayCommand(AddEmployee);
            EditEmployeeCommand = new RelayCommand<EmployeeModel>(EditEmployee, CanEditEmployee);
            RefreshCommand = new RelayCommand(RefreshEmployees);
            NextPageCommand = new RelayCommand(NextPage, CanNextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);
            GoToPageCommand = new RelayCommand<int>(GoToPage);

            // Load employees on initialization
            LoadEmployees();
        }

        #region Properties
        public ObservableCollection<EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeModel> FilteredEmployees
        {
            get => _filteredEmployees;
            set
            {
                _filteredEmployees = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<EmployeeModel> PaginatedEmployees
        {
            get => _paginatedEmployees;
            set
            {
                _paginatedEmployees = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterEmployees();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdatePagination();
                UpdatePageCommands();
            }
        }

        public int TotalPages => (int)Math.Ceiling((double)FilteredEmployees?.Count / ItemsPerPage);

        public ObservableCollection<int> PageNumbers
        {
            get
            {
                var pages = new ObservableCollection<int>();
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
                return pages;
            }
        }
        #endregion

        #region Commands
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        #endregion

        #region Methods
        private void LoadEmployees()
        {
            try
            {
                Employees.Clear();

                using (var dbConnection = new DbConnections())
                {
                    var dataTable = new DataTable();
                    string query = "SELECT staff_id, name, age, gender, jobTitle, Department, phoneNumber FROM dbo.tbl_Staff ORDER BY name";

                    dbConnection.readDatathroughAdapter(query, dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var employee = new EmployeeModel
                        {
                            Id = row["staff_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["staff_id"]),
                            Name = row["name"] == DBNull.Value ? string.Empty : row["name"].ToString(),
                            Age = row["age"] == DBNull.Value ? 0 : Convert.ToInt32(row["age"]),
                            Gender = row["gender"] == DBNull.Value ? string.Empty : row["gender"].ToString(),
                            JobTitle = row["jobTitle"] == DBNull.Value ? string.Empty : row["jobTitle"].ToString(),
                            Department = row["Department"] == DBNull.Value ? string.Empty : row["Department"].ToString(),
                            PhoneNumber = row["phoneNumber"] == DBNull.Value ? string.Empty : row["phoneNumber"].ToString()
                        };
                        Employees.Add(employee);
                    }
                }

                FilterEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Database Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterEmployees()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredEmployees = new ObservableCollection<EmployeeModel>(Employees);
            }
            else
            {
                var filtered = Employees.Where(e =>
                    e.Name.ToLower().Contains(SearchText.ToLower()) ||
                    e.Department.ToLower().Contains(SearchText.ToLower()) ||
                    e.JobTitle.ToLower().Contains(SearchText.ToLower()) ||
                    e.PhoneNumber.Contains(SearchText)
                ).ToList();

                FilteredEmployees = new ObservableCollection<EmployeeModel>(filtered);
            }

            CurrentPage = 1; // Reset to first page when filtering
        }

        private void UpdatePagination()
        {
            if (FilteredEmployees == null) return;

            var startIndex = (CurrentPage - 1) * ItemsPerPage;
            var items = FilteredEmployees.Skip(startIndex).Take(ItemsPerPage).ToList();
            PaginatedEmployees = new ObservableCollection<EmployeeModel>(items);

            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(PageNumbers));
        }

        private void AddEmployee()
        {
            try
            {
                var addForm = new AddEmployeeFormView();
                addForm.Owner = Application.Current.MainWindow;
                addForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var result = addForm.ShowDialog();
                if (result == true)
                {
                    // Refresh the employee list after successful addition
                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add employee form: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanEditEmployee(EmployeeModel employee)
        {
            return employee != null;
        }

        private void EditEmployee(EmployeeModel employee)
        {
            if (employee != null)
            {
                try
                {
                    var editForm = new EditEmployeeFormView(employee.Id);
                    editForm.Owner = Application.Current.MainWindow;
                    editForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    var result = editForm.ShowDialog();
                    if (result == true)
                    {
                        // Refresh the employee list after successful edit
                        LoadEmployees();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening edit employee form: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshEmployees()
        {
            LoadEmployees();
        }

        #region Pagination Methods
        private bool CanNextPage()
        {
            return CurrentPage < TotalPages;
        }

        private void NextPage()
        {
            if (CanNextPage())
            {
                CurrentPage++;
            }
        }

        private bool CanPreviousPage()
        {
            return CurrentPage > 1;
        }

        private void PreviousPage()
        {
            if (CanPreviousPage())
            {
                CurrentPage--;
            }
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void UpdatePageCommands()
        {
            ((RelayCommand)NextPageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousPageCommand).RaiseCanExecuteChanged();
        }
        #endregion
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}