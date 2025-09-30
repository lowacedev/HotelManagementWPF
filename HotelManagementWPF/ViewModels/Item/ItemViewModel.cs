using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Inventory.Items;

namespace HotelManagementWPF.ViewModels.Item
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private readonly DbConnections _dbConnection;
        private ObservableCollection<Models.Item> _allItems;
        private ObservableCollection<Models.Item> _paginatedItems;
        private ObservableCollection<int> _pageNumbers;
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _totalItems = 0;
        private bool _isLoading;

        private const int ItemsPerPage = 10;

        public ItemViewModel()
        {
            _dbConnection = new DbConnections();
            _allItems = new ObservableCollection<Models.Item>();
            _paginatedItems = new ObservableCollection<Models.Item>();
            _pageNumbers = new ObservableCollection<int>();

            // Initialize commands
            AddItemCommand = new RelayCommand(OpenAddItemForm);
            EditItemCommand = new RelayCommand<Models.Item>(OpenEditItemForm, item => item != null);
            PreviousPageCommand = new RelayCommand(() => GoToPage(CurrentPage - 1), () => CurrentPage > 1);
            NextPageCommand = new RelayCommand(() => GoToPage(CurrentPage + 1), () => CurrentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(GoToPage);

            LoadItemsAsync();
        }

        #region Properties

        public ObservableCollection<Models.Item> PaginatedUsers => _paginatedItems;

        public ObservableCollection<int> PageNumbers => _pageNumbers;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterAndPaginateItems();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagination();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        #endregion

        #region Commands

        public ICommand AddItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        #endregion

        #region Command Methods

        private void OpenAddItemForm()
        {
            try
            {
                var addItemWindow = new AddItemFormView
                {
                    Owner = Application.Current.MainWindow
                };

                addItemWindow.ShowDialog();
                // Always refresh after dialog closes, regardless of result
                LoadItemsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add item form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenEditItemForm(Models.Item item)
        {
            try
            {
                var editItemWindow = new EditItemFormView(item)
                {
                    Owner = Application.Current.MainWindow
                };

                editItemWindow.ShowDialog();
                // Always refresh after dialog closes, regardless of result
                LoadItemsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening edit item form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
            }
        }

        #endregion

        #region Data Loading and Filtering

        private async void LoadItemsAsync()
        {
            try
            {
                IsLoading = true;
                await Task.Run(() =>
                {
                    var dataTable = new DataTable();
                    string query = @"
                        SELECT 
                            i.item_id, 
                            i.supplier_id, 
                            i.itemName, 
                            i.quantity, 
                            i.stockLevel,
                            s.name as Name
                        FROM tbl_Inventory_item i
                        INNER JOIN tbl_Supplier s ON i.supplier_id = s.supplier_id
                        ORDER BY i.itemName";

                    _dbConnection.readDatathroughAdapter(query, dataTable);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _allItems.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            _allItems.Add(new Models.Item
                            {
                                ItemId = Convert.ToInt32(row["item_id"]),
                                SupplierId = Convert.ToInt32(row["supplier_id"]),
                                ItemName = row["itemName"].ToString() ?? string.Empty,
                                Quantity = Convert.ToInt32(row["quantity"]),
                                StockLevel = row["stockLevel"].ToString() ?? string.Empty,
                                Name = row["name"].ToString() ?? string.Empty
                            });
                        }

                        FilterAndPaginateItems();
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterAndPaginateItems()
        {
            var filteredItems = _allItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filteredItems = filteredItems.Where(item =>
                    item.ItemName.ToLower().Contains(searchLower) ||
                    item.Name.ToLower().Contains(searchLower) ||
                    item.StockLevel.ToLower().Contains(searchLower));
            }

            var filteredList = filteredItems.ToList();
            TotalItems = filteredList.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = 1;
            }

            UpdatePagination();
        }

        private void UpdatePagination()
        {
            var filteredItems = _allItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filteredItems = filteredItems.Where(item =>
                    item.ItemName.ToLower().Contains(searchLower) ||
                    item.Name.ToLower().Contains(searchLower) ||
                    item.StockLevel.ToLower().Contains(searchLower));
            }

            var filteredList = filteredItems.ToList();
            var paginatedList = filteredList
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            _paginatedItems.Clear();
            foreach (var item in paginatedList)
            {
                _paginatedItems.Add(item);
            }

            UpdatePageNumbers();
        }

        private void UpdatePageNumbers()
        {
            _pageNumbers.Clear();

            if (TotalPages <= 0) return;

            int startPage = Math.Max(1, CurrentPage - 2);
            int endPage = Math.Min(TotalPages, CurrentPage + 2);

            for (int i = startPage; i <= endPage; i++)
            {
                _pageNumbers.Add(i);
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}