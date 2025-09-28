using System.Windows;
using System.Windows.Controls;

namespace HotelManagementWPF.Views.Booking
{
    public partial class BookingFormView : Window
    {
        public BookingFormView()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


}
