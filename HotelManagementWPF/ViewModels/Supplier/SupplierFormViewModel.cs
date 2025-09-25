using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels.Supplier
{
    public class SupplierFormViewModel : INotifyPropertyChanged
    {
        private readonly IWindowService _windowService;
        private string _searchText;
        private ObservableCollection<SupplierData> _suppliers;
        private ObservableCollection<SupplierData> _paginatedSuppliers;
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

        public ObservableCollection<SupplierData> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged();
                FilterAndPaginateSuppliers();
            }
        }

        public ObservableCollection<SupplierData> PaginatedUsers // Keep the same name as in XAML
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
        public ICommand EditUserCommand { get; } // Keep the same name as in XAML
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        public SupplierFormViewModel(IWindowService windowService = null)
        {
            _windowService = windowService ?? new WindowService();

            // Initialize commands - using the correct constructor signature
            AddSupplierCommand = new RelayCommand<object>(ExecuteAddSupplier);
            EditUserCommand = new RelayCommand<object>(ExecuteEditSupplier);
            PreviousPageCommand = new RelayCommand<object>(ExecutePreviousPage, CanExecutePreviousPage);
            NextPageCommand = new RelayCommand<object>(ExecuteNextPage, CanExecuteNextPage);
            GoToPageCommand = new RelayCommand<object>(ExecuteGoToPage);

            // Initialize sample data
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            Suppliers = new ObservableCollection<SupplierData>
            {
                new SupplierData { supplierName = "ABC Suppliers", contactPerson = "John Smith", Location = "New York", phonenumber = "123-456-7890" },
                new SupplierData { supplierName = "XYZ Trading", contactPerson = "Jane Doe", Location = "California", phonenumber = "987-654-3210" },
                new SupplierData { supplierName = "Global Supply Co.", contactPerson = "Mike Johnson", Location = "Texas", phonenumber = "555-123-4567" },
                // Add more sample data as needed
            };
        }

        private void ExecuteAddSupplier(object? parameter)
        {
            _windowService.ShowAddSupplierForm();
            // After the form is closed, you might want to refresh the list
            // For now, we'll just keep the existing data
        }

        private void ExecuteEditSupplier(object? parameter)
        {
            if (parameter is SupplierData supplier)
            {
                _windowService.ShowEditSupplierForm(supplier);
                // After editing, you might want to refresh the list
                // For now, the changes are applied directly to the object reference
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
                    s.supplierName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    s.contactPerson?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    s.phonenumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    s.Location?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }

            var filteredList = filteredSuppliers.ToList();

            // Calculate pagination
            TotalPages = (int)Math.Ceiling((double)filteredList.Count / _itemsPerPage);
            if (TotalPages == 0) TotalPages = 1;

            // Ensure current page is valid
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            // Get items for current page
            var paginatedItems = filteredList
                .Skip((CurrentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            PaginatedUsers = new ObservableCollection<SupplierData>(paginatedItems);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Data model for suppliers
    public class SupplierData
    {
        public string supplierName { get; set; }
        public string contactPerson { get; set; }
        public string Location { get; set; }
        public string phonenumber { get; set; }
    }


}