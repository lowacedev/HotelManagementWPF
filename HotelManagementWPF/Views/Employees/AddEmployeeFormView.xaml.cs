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
using HotelManagementWPF.ViewModels.Employee;

namespace HotelManagementWPF.Views.Employees
{
    /// <summary>
    /// Interaction logic for AddEmployeeFormView.xaml
    /// </summary>
    public partial class AddEmployeeFormView : Window
    {
        public AddEmployeeFormView()
        {
            InitializeComponent();
            DataContext = new AddEmployeeFormViewModel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}