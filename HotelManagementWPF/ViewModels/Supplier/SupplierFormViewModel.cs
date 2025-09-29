using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels.Base;
using DatabaseProject;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.ViewModels.Supplier
{
    public class SupplierFormViewModel : INotifyPropertyChanged
    {
        private readonly IWindowService _windowService;
        private string _searchText;
        private ObservableCollection<Models.Supplier> _suppliers;
        private ObservableCollection<Models.Supplier> _paginatedSuppliers;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterAndPaginateSuppliers();
            }
        }

        public ObservableCollection<Models.Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged();
                FilterAndPaginateSuppliers();
            }
        }

        public ObservableCollection<Models.Supplier> PaginatedUsers
        {
            get => _paginatedSuppliers;
            set
            {
                _paginatedSuppliers = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                FilterAndPaginateSuppliers();
                OnPropertyChanged(nameof(PageNumbers));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageNumbers));
            }
        }

        public List<int> PageNumbers
        {
            get
            {
                var pages = new List<int>();
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
                return pages;
            }
        }

        // Commands
        public ICommand AddSupplierCommand { get; }
        public ICommand EditUserCommand { get; } // Kept as in XAML
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand RefreshCommand { get; }

        public SupplierFormViewModel(IWindowService windowService = null)
        {
            _windowService = windowService ?? new WindowService();

            // Initialize commands
            AddSupplierCommand = new RelayCommand<object>(ExecuteAddSupplier);
            EditUserCommand = new RelayCommand<object>(ExecuteEditSupplier);
            PreviousPageCommand = new RelayCommand<object>(ExecutePreviousPage, CanExecutePreviousPage);
            NextPageCommand = new RelayCommand<object>(ExecuteNextPage, CanExecuteNextPage);
            GoToPageCommand = new RelayCommand<object>(ExecuteGoToPage);
            RefreshCommand = new RelayCommand<object>(param => LoadSuppliersFromDatabase());

            // Load data from database
            LoadSuppliersFromDatabase();
        }

        private void LoadSuppliersFromDatabase()
        {
            try
            {
                using (var db = new DbConnections())
                {
                    DataTable dt = new DataTable();
                    string query = "SELECT supplier_id, name, location, phoneNumber FROM dbo.tbl_Supplier ORDER BY name";
                    db.readDatathroughAdapter(query, dt);

                    var supplierList = new ObservableCollection<Models.Supplier>();

                    foreach (DataRow row in dt.Rows)
                    {
                        supplierList.Add(new Models.Supplier
                        {
                            SupplierId = Convert.ToInt32(row["supplier_id"]),
                            Name = row["name"].ToString(),
                            Location = row["location"] != DBNull.Value ? row["location"].ToString() : string.Empty,
                            PhoneNumber = row["phoneNumber"] != DBNull.Value ? row["phoneNumber"].ToString() : string.Empty
                        });
                    }

                    Suppliers = supplierList;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading suppliers: {ex.Message}",
                                              "Error",
                                              System.Windows.MessageBoxButton.OK,
                                              System.Windows.MessageBoxImage.Error);

                Suppliers = new ObservableCollection<Models.Supplier>();
            }
        }

        private void ExecuteAddSupplier(object? parameter)
        {
            _windowService.ShowAddSupplierForm();
            LoadSuppliersFromDatabase();
        }

        private void ExecuteEditSupplier(object? parameter)
        {
            if (parameter is Models.Supplier supplier)
            {
                _windowService.ShowEditSupplierForm(supplier);
                LoadSuppliersFromDatabase();
            }
        }

        private void ExecutePreviousPage(object? parameter)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private bool CanExecutePreviousPage(object? parameter)
        {
            return CurrentPage > 1;
        }

        private void ExecuteNextPage(object? parameter)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private bool CanExecuteNextPage(object? parameter)
        {
            return CurrentPage < TotalPages;
        }

        private void ExecuteGoToPage(object? parameter)
        {
            if (parameter is int page && page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
            }
        }

        private void FilterAndPaginateSuppliers()
        {
            if (Suppliers == null) return;

            var filteredSuppliers = Suppliers.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filteredSuppliers = filteredSuppliers.Where(s =>
                    s.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    s.PhoneNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    s.Location?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }

            var filteredList = filteredSuppliers.ToList();

            // Pagination
            TotalPages = (int)Math.Ceiling((double)filteredList.Count / _itemsPerPage);
            if (TotalPages == 0) TotalPages = 1;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var paginatedItems = filteredList
                .Skip((CurrentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            PaginatedUsers = new ObservableCollection<Models.Supplier>(paginatedItems);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
