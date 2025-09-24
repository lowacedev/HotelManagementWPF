using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HotelManagementWPF.Views.Users
{
    /// <summary>
    /// Interaction logic for AddUserFormView.xaml
    /// </summary>
    public partial class AddUserFormView : Window
    {
        public AddUserFormView()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool isPasswordVisible = false;
        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}