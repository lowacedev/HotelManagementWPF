using System.Windows;
using HotelManagementWPF.ViewModels.Guest;
using HotelManagementWPF.ViewModels;

namespace HotelManagementWPF.Views.Guest
{
    /// <summary>
    /// Interaction logic for EditGuestFormView.xaml
    /// </summary>
    public partial class EditGuestFormView : Window
    {
        public EditGuestFormView()
        {
            InitializeComponent();
        }

        public EditGuestFormView(GuestModel guest) : this()
        {
            DataContext = new EditGuestFormViewModel(guest.Id);
        }
    }
}