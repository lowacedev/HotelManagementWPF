using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DatabaseProject; // ✅ for DbConnections
using HotelManagementWPF.ViewModels.Base; // ✅ for RelayCommand

namespace HotelManagementWPF.ViewModels.Employee
{
    internal class AddEmployeeFormViewModel : INotifyPropertyChanged
    {
        private string _name;
        private int _age;
        private string _gender;
        private string _jobTitle;
        private string _department;
        private string _phoneNumber;

        public AddEmployeeFormViewModel()
        {
            AddEmployeeCommand = new RelayCommand<object>(AddEmployee, CanAddEmployee);

            // Initialize gender options
            GenderOptions = new ObservableCollection<string>
            {
                "Male",
                "Female",
                "Other"
            };

            // Initialize department options
            DepartmentOptions = new ObservableCollection<string>
            {
                "Front Desk",
                "Housekeeping",
                "Food & Beverage",
                "Maintenance",
                "Security",
                "Management",
                "Kitchen",
                "Concierge"
            };

            // Initialize job title options
            JobTitleOptions = new ObservableCollection<string>
            {
                "Manager",
                "Assistant Manager",
                "Receptionist",
                "Housekeeper",
                "Waiter/Waitress",
                "Chef",
                "Security Guard",
                "Maintenance Worker",
                "Concierge",
                "Bellhop"
            };

            // Default values
            Gender = string.Empty;
            Department = string.Empty;
            JobTitle = string.Empty;
            
        }

        #region Properties
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ((RelayCommand<object>)AddEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
                ((RelayCommand<object>)AddEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public string JobTitle
        {
            get => _jobTitle;
            set
            {
                _jobTitle = value;
                OnPropertyChanged();
                ((RelayCommand<object>)AddEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
                ((RelayCommand<object>)AddEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                ((RelayCommand<object>)AddEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GenderOptions { get; set; }
        public ObservableCollection<string> DepartmentOptions { get; set; }
        public ObservableCollection<string> JobTitleOptions { get; set; }
        #endregion

        #region Commands
        public ICommand AddEmployeeCommand { get; }
        #endregion

        #region Methods
        private bool CanAddEmployee(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   Age > 0 &&
                   !string.IsNullOrWhiteSpace(JobTitle) &&
                   !string.IsNullOrWhiteSpace(Department) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private void AddEmployee(object parameter)
        {
            try
            {
                using (var db = new DbConnections())
                {
                    string query = @"INSERT INTO dbo.tbl_Staff 
                                    (name, age, gender, jobTitle, Department, phoneNumber) 
                                     VALUES (@Name, @Age, @Gender, @JobTitle, @Department, @PhoneNumber)";

                    var parameters = new System.Collections.Generic.Dictionary<string, object>
                    {
                        {"@Name", Name ?? string.Empty},
                        {"@Age", Age},
                        {"@Gender", Gender ?? string.Empty},
                        {"@JobTitle", JobTitle ?? string.Empty},
                        {"@Department", Department ?? string.Empty},
                        {"@PhoneNumber", PhoneNumber ?? string.Empty}
                    };

                    db.ExecuteNonQuery(query, parameters);

                    MessageBox.Show("Employee added successfully!", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form
                    ClearForm();

                    // Close window if passed
                    if (parameter is Window window)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Age = 0;              // Add this line
            Gender = string.Empty; // Fix: was null, should be string.Empty
            JobTitle = string.Empty;  // Add this line
            Department = string.Empty;
            PhoneNumber = string.Empty;
        }
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
