using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using HotelManagementWPF.ViewModels.Base;
using DatabaseProject;

namespace HotelManagementWPF.ViewModels.Employee
{
    internal class EditEmployeeFormViewModel : INotifyPropertyChanged
    {
        private int _staffId;
        private string _name;
        private int _age;
        private string _gender;
        private string _jobTitle;
        private string _department;
        private string _phoneNumber;

        // Remove connection string since we're using DbConnections class

        public EditEmployeeFormViewModel(int staffId)
        {
            _staffId = staffId;

            UpdateEmployeeCommand = new RelayCommand<Window>(UpdateEmployee, CanUpdateEmployee);

            // Initialize options
            GenderOptions = new ObservableCollection<string>
            {
                "Male",
                "Female",
                "Other"
            };

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

            // Load existing employee data
            LoadEmployeeData();
        }

        #region Properties
        public int StaffId
        {
            get => _staffId;
            set
            {
                _staffId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ((RelayCommand<Window>)UpdateEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
                ((RelayCommand<Window>)UpdateEmployeeCommand).RaiseCanExecuteChanged();
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
                ((RelayCommand<Window>)UpdateEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
                ((RelayCommand<Window>)UpdateEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                ((RelayCommand<Window>)UpdateEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GenderOptions { get; set; }
        public ObservableCollection<string> DepartmentOptions { get; set; }
        public ObservableCollection<string> JobTitleOptions { get; set; }
        #endregion

        #region Commands
        public ICommand UpdateEmployeeCommand { get; }
        #endregion

        #region Methods
        private void LoadEmployeeData()
        {
            try
            {
                using (var dbConnection = new DbConnections())
                {
                    var dataTable = new DataTable();
                    string query = @"SELECT name, age, gender, jobTitle, Department, phoneNumber 
                           FROM dbo.tbl_Staff 
                           WHERE staff_id = @StaffId";

                    var parameters = new Dictionary<string, object>
            {
                { "@StaffId", _staffId }
            };

                    dbConnection.readDataWithParameters(query, dataTable, parameters);

                    if (dataTable.Rows.Count > 0)
                    {
                        var row = dataTable.Rows[0];
                        Name = row["name"] == DBNull.Value ? string.Empty : row["name"].ToString();
                        Age = row["age"] == DBNull.Value ? 0 : Convert.ToInt32(row["age"]);
                        Gender = row["gender"] == DBNull.Value ? string.Empty : row["gender"].ToString();
                        JobTitle = row["jobTitle"] == DBNull.Value ? string.Empty : row["jobTitle"].ToString();
                        Department = row["Department"] == DBNull.Value ? string.Empty : row["Department"].ToString();
                        PhoneNumber = row["phoneNumber"] == DBNull.Value ? string.Empty : row["phoneNumber"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee data: {ex.Message}", "Database Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdateEmployee(Window parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   Age > 0 &&
                   !string.IsNullOrWhiteSpace(JobTitle) &&
                   !string.IsNullOrWhiteSpace(Department) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);
        }

        private void UpdateEmployee(Window parameter)
        {
            try
            {
                using (var dbConnection = new DbConnections())
                {
                    string query = @"UPDATE dbo.tbl_Staff 
                                   SET name = @Name, 
                                       age = @Age, 
                                       gender = @Gender, 
                                       jobTitle = @JobTitle, 
                                       Department = @Department, 
                                       phoneNumber = @PhoneNumber
                                   WHERE staff_id = @StaffId";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@StaffId", _staffId },
                        { "@Name", Name ?? string.Empty },
                        { "@Age", Age },
                        { "@Gender", Gender ?? string.Empty },
                        { "@JobTitle", JobTitle ?? string.Empty },
                        { "@Department", Department ?? string.Empty },
                        { "@PhoneNumber", PhoneNumber ?? string.Empty }
                    };

                    dbConnection.ExecuteNonQuery(query, parameters);

                    MessageBox.Show("Employee updated successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    // Close the window
                    if (parameter != null)
                    {
                        parameter.DialogResult = true;
                        parameter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee: {ex.Message}", "Database Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
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