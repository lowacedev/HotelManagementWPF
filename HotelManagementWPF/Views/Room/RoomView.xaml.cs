using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels;
using System.Windows.Controls;

namespace HotelManagementWPF.Views.Room
{
    public partial class RoomView : UserControl
    {
        public RoomView()
        {
            InitializeComponent();
            var windowService = new WindowService();
            DataContext = new RoomViewModel(windowService);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Optional: handle button clicks if needed
        }
    }
}