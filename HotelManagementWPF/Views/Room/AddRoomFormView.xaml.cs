using HotelManagementWPF.ViewModels;
using HotelManagementWPF.ViewModels.Room;
using System.Windows;

namespace HotelManagementWPF.Views.Room
{
public partial class AddRoomFormView : Window
{
    private readonly RoomViewModel _mainViewModel;

    public AddRoomFormView(RoomViewModel mainViewModel, Action onRoomAdded)
    {
        InitializeComponent();
        _mainViewModel = mainViewModel;

        // Create ViewModel with callback
        var vm = new AddRoomViewModel(_mainViewModel, onRoomAdded);
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