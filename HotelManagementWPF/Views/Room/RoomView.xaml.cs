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
            DataContext = new RoomViewModel(new WindowService());
        }
    }
}
