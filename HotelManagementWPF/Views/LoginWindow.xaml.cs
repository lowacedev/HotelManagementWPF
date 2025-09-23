using System.Windows;
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;

            // Basic validation
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter your username.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                if (_isPasswordVisible)
                    PasswordTextBox.Focus();
                else
                    PasswordBox.Focus();
                return;
            }

            // TODO: Implement your login logic here
            // For now, just show a placeholder message
            MessageBox.Show($"Login attempted with:\nUsername: {username}\nPassword: {new string('*', password.Length)}",
                "Login Info", MessageBoxButton.OK, MessageBoxImage.Information);

            // Example: Close login window and show main window
            // this.DialogResult = true;
            // this.Close();
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