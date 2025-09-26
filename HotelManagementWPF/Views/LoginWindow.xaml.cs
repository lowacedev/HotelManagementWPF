using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            // Set initial focus to username textbox
            this.Loaded += (sender, e) => UsernameTextBox.Focus();

            // Add Enter key support for login
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
                // Show password as text
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Focus();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;

                // Change eye icon opacity to indicate active state
                EyeIcon.Opacity = 1.0;
            }
            else
            {
                // Hide password
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Focus();

                // Reset eye icon opacity
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

                    string query = "SELECT role FROM tbl_User WHERE username = @username AND password = @password";
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

                    string role = dt.Rows[0]["role"].ToString();

                    // Based on role, open main window with permissions
                    var mainWindow = new MainWindow(role);
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
            // TODO: Implement forgot password functionality
            MessageBox.Show("Forgot password functionality will be implemented here.",
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContactSupport_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement contact support functionality
            MessageBox.Show("Contact support functionality will be implemented here.",
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}