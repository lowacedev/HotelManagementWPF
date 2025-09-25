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
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string BedType { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public EditRoomFormView(int roomId)
        {
            InitializeComponent();
            this.DataContext = new HotelManagementWPF.ViewModels.EditRoomViewModel(roomId);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
