using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels;
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

namespace HotelManagementWPF.Views.Guest
{
    /// <summary>
    /// Interaction logic for GuestView.xaml
    /// </summary>
    public partial class GuestView : UserControl
    {
        public GuestView()
        {
            InitializeComponent();
            DataContext = new GuestViewModel(new WindowService());

        }

        private void AddGuestButton_Click(object sender, RoutedEventArgs e)
        {
            // This will trigger both the Click event and Command if both are set
            // The Command binding will handle the actual logic
        }
    }
}
