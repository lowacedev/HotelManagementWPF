using HotelManagementWPF.Models;
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

namespace HotelManagementWPF.Views.Room
{
    /// <summary>
    /// Interaction logic for EditRoomFormView.xaml
    /// </summary>
    public partial class EditRoomFormView : Window
    {
        public event Action RoomUpdated; // Event to notify update

        public int RoomId { get; set; }

        public EditRoomFormView(int roomId)
        {
            InitializeComponent();
            this.DataContext = new HotelManagementWPF.ViewModels.EditRoomViewModel(roomId);
            this.RoomId = roomId;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Call ViewModel's save method
            if (DataContext is HotelManagementWPF.ViewModels.EditRoomViewModel vm)
            {
                vm.SaveChanges();

                // Set DialogResult to true to indicate success
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
