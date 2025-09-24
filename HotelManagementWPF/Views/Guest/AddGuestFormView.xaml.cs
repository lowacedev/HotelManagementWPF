using HotelManagementWPF.ViewModels.Guest; // If using ViewModels
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementWPF.Views.Guest
{
    public partial class AddGuestFormView : Window
    {
        public AddGuestFormView()
        {
            InitializeComponent();
            // Set DataContext if using MVVM
            // this.DataContext = new AddGuestViewModel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Collect data from controls
            string name = NameTextBox.Text.Trim();
            string ageText = AgeTextBox.Text.Trim();
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string phoneNumber = PhoneNumberTextBox.Text.Trim();

            // Basic validation
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter the guest's name.", "Validation Error");
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                MessageBox.Show("Please enter a valid age.", "Validation Error");
                return;
            }

            if (string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Please select gender.", "Validation Error");
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter the phone number.", "Validation Error");
                return;
            }

            // Call the insert method
            try
            {
                InsertGuest(name, age, gender, phoneNumber);
                MessageBox.Show("Guest added successfully!");
                this.Close(); // Close window after success
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void InsertGuest(string name, int age, string gender, string phoneNumber)
        {
            string query = @"INSERT INTO tbl_Guest (name, age, gender, phoneNumber)
                             VALUES (@name, @age, @gender, @phoneNumber)";

            var parameters = new Dictionary<string, object>
            {
                { "@name", name },
                { "@age", age },
                { "@gender", gender },
                { "@phoneNumber", phoneNumber }
            };

            var db = new DatabaseProject.DbConnections();
            try
            {
                db.createConn();
                db.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to insert guest: " + ex.Message);
            }
            finally
            {
                db.closeConn();
            }
        }
    }
}