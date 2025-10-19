using System.Windows;
using System.Windows.Controls;
using HotelManagementWPF.ViewModels;

namespace HotelManagementWPF.Views.Users
{
    public partial class UserView : UserControl
    {
        private UserViewModel _viewModel;
        private bool _hasLoaded = false;

        public UserView(UserViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            this.Loaded += UserView_Loaded;
        }

        private void UserView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_hasLoaded)
            {
                _hasLoaded = true;
                _viewModel.OnViewLoaded();
            }
        }
    }
}