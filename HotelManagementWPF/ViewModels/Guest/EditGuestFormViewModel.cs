using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels.Guest
{
    public class EditGuestFormViewModel : INotifyPropertyChanged
    {
        private readonly int _guest_id;
        private readonly DbConnections _db;

        private string _name;
        private int _age;
        private string _gender;
        private string _phoneNumber;

        public List<string> GenderOptions { get; } = new List<string>
        {
            "Male", "Female", "Other"
        };

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EditGuestFormViewModel(int guest_id)
        {
            _guest_id = guest_id;
            _db = new DbConnections();

            LoadGuestFromDatabase();

            SaveChangesCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void LoadGuestFromDatabase()
        {
            var dt = new DataTable();
            string query = $"SELECT * FROM tbl_Guest WHERE guest_id = {_guest_id}";
            _db.readDatathroughAdapter(query, dt);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                Name = row["name"].ToString();
                Age = Convert.ToInt32(row["age"]);
                Gender = row["gender"].ToString();
                PhoneNumber = row["phoneNumber"].ToString();
            }
            else
            {
                MessageBox.Show("Guest not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChanges()
        {
            try
            {
                // Validate age input
                if (Age <= 0)
                {
                    MessageBox.Show("Please enter a valid age.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Gender) ||
                    string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string updateQuery = @"
                    UPDATE tbl_Guest SET
                        name = @Name,
                        age = @Age,
                        gender = @Gender,
                        phoneNumber = @PhoneNumber
                    WHERE guest_id = @GuestId";

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", Name },
                    { "@Age", Age },
                    { "@Gender", Gender },
                    { "@PhoneNumber", PhoneNumber },
                    { "@GuestId", _guest_id }
                };

                _db.ExecuteNonQuery(updateQuery, parameters);

                MessageBox.Show($"Guest {Name} updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.DataContext == this)
                    {
                        w.DialogResult = true;
                        w.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating guest: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            // Close the window with DialogResult = false
            foreach (Window w in Application.Current.Windows)
            {
                if (w.DataContext == this)
                {
                    w.DialogResult = false;
                    w.Close();
                    break;
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}