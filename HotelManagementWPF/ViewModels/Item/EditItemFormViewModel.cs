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
using SupplierModel = HotelManagementWPF.Models.Supplier;

namespace HotelManagementWPF.ViewModels.Item
{
    public class EditItemFormViewModel : INotifyPropertyChanged
    {
        private readonly DbConnections _dbConnection;
        private readonly Models.Item _originalItem;
        private int _itemId;
        private string _itemName = string.Empty;
        private int _quantity;
        private string _stockLevel = string.Empty;
        private int _supplierId;
        private ObservableCollection<SupplierModel> _suppliers;
        private bool _isLoading;

        public EditItemFormViewModel(Models.Item itemToEdit)
        {
            _dbConnection = new DbConnections();
            _originalItem = itemToEdit;
            _suppliers = new ObservableCollection<SupplierModel>();

            // Initialize with existing item data
            _itemId = itemToEdit.ItemId;
            _itemName = itemToEdit.ItemName;
            _quantity = itemToEdit.Quantity;
            _stockLevel = itemToEdit.StockLevel;
            _supplierId = itemToEdit.SupplierId;

            UpdateItemCommand = new RelayCommand<Window>(async (window) => await UpdateItemAsync(window));

            LoadSuppliersAsync();
        }

        public string WindowTitle => "Edit Item";

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

        public int SupplierId
        {
            get => _supplierId;
            set
            {
                _supplierId = value;
                OnPropertyChanged(nameof(SupplierId));
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

        public ICommand UpdateItemCommand { get; }

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

        private async Task UpdateItemAsync(Window window)
        {
            if (!ValidateInput())
                return;

            try
            {
                IsLoading = true;

                var parameters = new Dictionary<string, object>
                {
                    { "@item_id", _itemId },
                    { "@supplier_id", SupplierId },
                    { "@itemName", ItemName.Trim() },
                    { "@quantity", Quantity },
                    { "@stockLevel", StockLevel.Trim() }
                };

                string query = @"UPDATE tbl_Inventory_item 
                                 SET supplier_id = @supplier_id, 
                                     itemName = @itemName, 
                                     quantity = @quantity, 
                                     stockLevel = @stockLevel 
                                 WHERE item_id = @item_id";

                await _dbConnection.ExecuteNonQueryAsync(query, parameters);

                MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                window?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (SupplierId <= 0)
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