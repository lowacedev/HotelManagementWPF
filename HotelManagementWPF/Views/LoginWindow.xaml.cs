using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();

            // Allow window dragging
            this.MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            };

            // Set focus to username textbox on load
            this.Loaded += (sender, e) => UsernameTextBox.Focus();

            // Support pressing Enter to login
            this.KeyDown += LoginWindow_KeyDown;
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Focus();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;

                EyeIcon.Opacity = 1.0;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Focus();

                EyeIcon.Opacity = 0.5;
            }
        }

        private void ClearErrorMessages()
        {
            UsernameErrorTextBlock.Visibility = Visibility.Collapsed;
            PasswordErrorTextBlock.Visibility = Visibility.Collapsed;
            UsernameErrorTextBlock.Text = "";
            PasswordErrorTextBlock.Text = "";
        }

        private void ShowError(TextBlock errorBlock, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous errors
            ClearErrorMessages();

            string username = UsernameTextBox.Text;
            string password = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError(UsernameErrorTextBlock, "Please input username");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError(PasswordErrorTextBlock, "Please input password");
                hasError = true;
            }

            if (hasError)
                return;

            // Validate credentials against database
            using (var db = new DatabaseProject.DbConnections())
            {
                try
                {
                    db.createConn();

                    // Make sure your table has user_id, role, name, username, password
                    string query = "SELECT user_id, role, name FROM tbl_User WHERE username = @username AND password = @password";
                    var parameters = new Dictionary<string, object>
                    {
                        {"@username", username},
                        {"@password", password}
                    };

                    DataTable dt = new DataTable();
                    db.readDataWithParameters(query, dt, parameters);

                    if (dt.Rows.Count == 0)
                    {
                        // Invalid credentials
                        ShowError(UsernameErrorTextBlock, "Invalid username or password");
                        ShowError(PasswordErrorTextBlock, "Invalid username or password");
                        return;
                    }

                    // Retrieve user info
                    int userId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                    string role = dt.Rows[0]["role"].ToString();
                    string fullName = dt.Rows[0]["name"].ToString();

                    // Set session user id
                    Session.CurrentUserId = userId;

                    // Debug: Verify user ID
                    Console.WriteLine($"Logged in user ID: {Session.CurrentUserId}");

                    // Open main window
                    var mainWindow = new MainWindow(role, fullName);
                    mainWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Forgot password functionality will be implemented here.",
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContactSupport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Contact support functionality will be implemented here.",
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Example: method to check if user is logged in before accessing user ID
        private void SomeOperationRequiringUserId()
        {
            if (Session.CurrentUserId <= 0)
            {
                MessageBox.Show("Invalid user ID. Please log in.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Optionally, redirect to login
                return;
            }

            // Safe to proceed with user-specific operation
        }
    }
}