using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels;
using System.Windows;

namespace HotelManagementWPF.Views.Room
{
    public partial class AddRoomFormView : Window
    {
        private readonly RoomViewModel _mainViewModel;

        public AddRoomFormView(RoomViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
           // this.DataContext = new AddRoomViewModel(_mainViewModel);


 
            var vm = new AddRoomViewModel(_mainViewModel);
            // Subscribe to close event
            vm.CloseAction += () => this.Dispatcher.Invoke(() => this.Close());
            this.DataContext = vm;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}