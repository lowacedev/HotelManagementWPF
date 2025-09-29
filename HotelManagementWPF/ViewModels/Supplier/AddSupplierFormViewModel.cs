using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;
using DatabaseProject; // for DbConnections

namespace HotelManagementWPF.ViewModels.Supplier
{
    public class AddSupplierFormViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _location;
        private string _phoneNumber;
 


        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                (AddSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
                (AddSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                (AddSupplierCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }

       


        public ICommand AddSupplierCommand { get; }

        public AddSupplierFormViewModel()
        {
            AddSupplierCommand = new RelayCommand<object>(ExecuteAddSupplier, CanExecuteAddSupplier);
        }

        private bool CanExecuteAddSupplier(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Location) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
                
        }

        private void ExecuteAddSupplier(object? parameter)
        {
            try
            {
                using (var db = new DbConnections())
                {
                    string query = @"INSERT INTO dbo.tbl_Supplier 
                                     (name, location, phoneNumber) 
                                     VALUES (@Name, @Location, @PhoneNumber)";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@Name", Name },
                        { "@Location", Location },
                        { "@PhoneNumber", PhoneNumber }
                       
                    };

                    db.ExecuteNonQuery(query, parameters);
                }

                MessageBox.Show($"Supplier '{Name}' has been added successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // Close the window if parameter is passed as Window
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
