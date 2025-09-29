using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;

// 👇 Alias so we always mean the Supplier model, not namespace
using SupplierModel = HotelManagementWPF.Models.Supplier;

namespace HotelManagementWPF.ViewModels.Item
{
    public class AddItemFormViewModel : INotifyPropertyChanged
    {
        private readonly DbConnections _dbConnection;
        private string _itemName = string.Empty;
        private int _quantity;
        private string _stockLevel = string.Empty;
        private SupplierModel _selectedSupplier;
        private ObservableCollection<SupplierModel> _suppliers;
        private bool _isLoading;

        public AddItemFormViewModel()
        {
            _dbConnection = new DbConnections();
            _suppliers = new ObservableCollection<SupplierModel>();

            AddItemCommand = new RelayCommand<Window>(async (window) => await AddItemAsync(window));

            LoadSuppliersAsync();
        }

        public string WindowTitle => "Add New Item";

        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string StockLevel
        {
            get => _stockLevel;
            set
            {
                _stockLevel = value;
                OnPropertyChanged(nameof(StockLevel));
            }
        }

        public SupplierModel SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        public ObservableCollection<SupplierModel> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
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

        public ICommand AddItemCommand { get; }

        private async void LoadSuppliersAsync()
        {
            try
            {
                IsLoading = true;
                await Task.Run(() =>
                {
                    var dataTable = new DataTable();
                    _dbConnection.readDatathroughAdapter("SELECT supplier_id, name, location, phoneNumber FROM tbl_Supplier ORDER BY name", dataTable);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Suppliers.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            Suppliers.Add(new SupplierModel
                            {
                                SupplierId = Convert.ToInt32(row["supplier_id"]),
                                Name = row["name"].ToString() ?? string.Empty,
                                Location = row["location"].ToString() ?? string.Empty,
                                PhoneNumber = row["phoneNumber"].ToString() ?? string.Empty
                            });
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddItemAsync(Window window)
        {
            if (!ValidateInput())
                return;

            try
            {
                IsLoading = true;

                var parameters = new Dictionary<string, object>
                {
                    { "@supplier_id", SelectedSupplier.SupplierId },
                    { "@itemName", ItemName.Trim() },
                    { "@quantity", Quantity },
                    { "@stockLevel", StockLevel.Trim() }
                };

                string query = @"INSERT INTO tbl_Inventory_item (supplier_id, itemName, quantity, stockLevel) 
                                 VALUES (@supplier_id, @itemName, @quantity, @stockLevel)";

                await _dbConnection.ExecuteNonQueryAsync(query, parameters);

                MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                window?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(ItemName))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedSupplier == null)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Quantity < 0)
            {
                MessageBox.Show("Quantity cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(StockLevel))
            {
                MessageBox.Show("Please enter a stock level.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
