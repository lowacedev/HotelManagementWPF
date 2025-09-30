using System.Windows.Controls;

namespace HotelManagementWPF.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel(); // Instantiate your ViewModel
            this.DataContext = _viewModel; // Set DataContext once

            // Trigger initial data load
            _ = _viewModel.RefreshDashboardDataAsync();
        }
    }
}