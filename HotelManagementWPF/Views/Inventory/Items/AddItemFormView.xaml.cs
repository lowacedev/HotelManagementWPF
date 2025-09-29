using System;
using System.Windows;
using HotelManagementWPF.ViewModels.Item;

namespace HotelManagementWPF.Views.Inventory.Items
{
    public partial class AddItemFormView : Window
    {
        public AddItemFormView()
        {
            InitializeComponent();
            DataContext = new AddItemFormViewModel();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}