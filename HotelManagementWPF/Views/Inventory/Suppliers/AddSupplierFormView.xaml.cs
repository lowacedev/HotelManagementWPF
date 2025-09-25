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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HotelManagementWPF.ViewModels.Supplier;

namespace HotelManagementWPF.Views.Inventory.Suppliers
{
    /// <summary>
    /// Interaction logic for AddSupplierFormView.xaml
    /// </summary>
    public partial class AddSupplierFormView : Window
    {
        public AddSupplierFormView()
        {
            InitializeComponent();
            DataContext = new AddSupplierFormViewModel();
        }

        public AddSupplierFormView(AddSupplierFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            // The command will handle the logic, but this can be used for additional validation
            var viewModel = DataContext as AddSupplierFormViewModel;
            if (viewModel != null && viewModel.CanAddSupplier())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}