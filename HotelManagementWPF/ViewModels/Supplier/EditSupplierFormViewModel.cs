using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;
using DatabaseProject; // for DbConnections
using SupplierModel = HotelManagementWPF.Models.Supplier;

namespace HotelManagementWPF.ViewModels.Supplier
{
    public class EditSupplierFormViewModel : INotifyPropertyChanged
    {
        private int _supplierId;
        private string _name;
        private string _location;
        private string _phoneNumber;

        public int SupplierId
        {
            get => _supplierId;
            set { _supplierId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                (UpdateSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
                (UpdateSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                (UpdateSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand UpdateSupplierCommand { get; }

        public EditSupplierFormViewModel(SupplierModel supplier)
        {
            // Fill fields with existing supplier data
            SupplierId = supplier.SupplierId;
            Name = supplier.Name;
            Location = supplier.Location;
            PhoneNumber = supplier.PhoneNumber;

            UpdateSupplierCommand = new RelayCommand<object>(ExecuteUpdateSupplier, CanExecuteUpdateSupplier);
        }

        private bool CanExecuteUpdateSupplier(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Location) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private void ExecuteUpdateSupplier(object? parameter)
        {
            try
            {
                using (var db = new DbConnections())
                {
                    string query = @"UPDATE dbo.tbl_Supplier
                                     SET name = @Name,
                                         location = @Location,
                                         phoneNumber = @PhoneNumber
                                     WHERE supplierId = @SupplierId";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@Name", Name },
                        { "@Location", Location },
                        { "@PhoneNumber", PhoneNumber },
                        { "@SupplierId", SupplierId }
                    };

                    db.ExecuteNonQuery(query, parameters);
                }

                MessageBox.Show($"Supplier '{Name}' has been updated successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating supplier: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
