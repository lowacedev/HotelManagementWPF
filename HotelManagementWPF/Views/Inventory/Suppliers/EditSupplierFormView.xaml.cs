using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Supplier;
using System.Windows;


namespace HotelManagementWPF.Views.Inventory.Suppliers
{
    public partial class EditSupplierFormView : Window
    {
        public EditSupplierFormView(Supplier supplier)
        {
            InitializeComponent();
            DataContext = new EditSupplierFormViewModel(supplier);
        }
    }
}
