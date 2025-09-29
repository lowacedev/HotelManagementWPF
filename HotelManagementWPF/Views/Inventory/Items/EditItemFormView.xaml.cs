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
using HotelManagementWPF.ViewModels.Item;

namespace HotelManagementWPF.Views.Inventory.Items
{
    /// <summary>
    /// Interaction logic for EditItemFormView.xaml
    /// </summary>
    public partial class EditItemFormView : Window
    {
        public EditItemFormView(Models.Item itemToEdit)
        {
            InitializeComponent();
            DataContext = new EditItemFormViewModel(itemToEdit);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}