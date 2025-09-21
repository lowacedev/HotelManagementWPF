using HotelManagementWPF.ViewModels;
using System.Windows;

namespace HotelManagementWPF.Views.Room
{
    public partial class EditRoomFormView : Window
    {
        public EditRoomFormView(Models.Room room)
        {
            InitializeComponent();
            DataContext = new EditRoomViewModel(room);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}