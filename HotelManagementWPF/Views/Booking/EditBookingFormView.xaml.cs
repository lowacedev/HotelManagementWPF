using HotelManagementWPF.ViewModels.Booking;
using System.Windows;

namespace HotelManagementWPF.Views.Booking
{
    /// <summary>
    /// Interaction logic for EditBookingFormView.xaml
    /// </summary>
    public partial class EditBookingFormView : Window
    {
        public EditBookingFormView(int bookingId)
        {
            InitializeComponent();

            // Set the DataContext to the EditBookingFormViewModel with the booking ID
            this.DataContext = new EditBookingFormViewModel(bookingId);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}