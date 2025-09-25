using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels.Supplier
{
    public class AddSupplierFormViewModel : INotifyPropertyChanged
    {
        private string _supplierName;
        private string _contactPerson;
        private string _location;
        private string _phoneNumber;

        public string SupplierName
        {
            get => _supplierName;
            set
            {
                _supplierName = value;
                OnPropertyChanged();
            }
        }

        public string ContactPerson
        {
            get => _contactPerson;
            set
            {
                _contactPerson = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSupplierCommand { get; }

        public AddSupplierFormViewModel()
        {
            AddSupplierCommand = new RelayCommand<object>(ExecuteAddSupplier, CanExecuteAddSupplier);
        }

        private bool CanExecuteAddSupplier(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(SupplierName) &&
                   !string.IsNullOrWhiteSpace(ContactPerson) &&
                   !string.IsNullOrWhiteSpace(Location) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private void ExecuteAddSupplier(object? parameter)
        {
            try
            {
                // Here you would typically call a service to save the supplier
                // For now, we'll just show a message
                MessageBox.Show($"Supplier '{SupplierName}' has been added successfully!", 
                               "Success", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Information);

                // Close the window if parameter is a Window
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding supplier: {ex.Message}", 
                               "Error", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
            }
        }

        public bool CanAddSupplier()
        {
            return CanExecuteAddSupplier(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation
 
}