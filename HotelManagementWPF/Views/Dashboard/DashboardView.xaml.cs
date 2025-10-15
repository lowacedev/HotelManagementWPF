using System.Windows.Controls;

namespace HotelManagementWPF.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            this.DataContext = _viewModel;

            // Call async initialization without awaiting in constructor
            _ = _viewModel.InitializeAsync();
        }
    }
}