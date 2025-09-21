using HotelManagementWPF.ViewModels;
using System.Windows;

namespace HotelManagementWPF.Views.Room
{
    public partial class AddRoomFormView : Window
    {
        public AddRoomFormView()
        {
            InitializeComponent();
       
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}