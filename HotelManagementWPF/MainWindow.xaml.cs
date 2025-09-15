using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Room;

namespace HotelManagementWPF
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarCollapsed = false;

        public MainWindow()
        {
            InitializeComponent();

            // Load default dashboard at startup
            NavigateToSection("dashboard");
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSidebar();
        }

        private void ToggleSidebar()
        {
            _isSidebarCollapsed = !_isSidebarCollapsed;

            SidebarColumn.Width = _isSidebarCollapsed
                ? new GridLength(60)
                : new GridLength(220);

            HeaderLogoText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            ManagementHeader.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            OperationsHeader.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            DashboardButton.Content = _isSidebarCollapsed ? null : "Dashboard";
            GuestsButton.Content = _isSidebarCollapsed ? null : "Guests";
            UsersButton.Content = _isSidebarCollapsed ? null : "Users";
            RoomsButton.Content = _isSidebarCollapsed ? null : "Rooms";
            BookingsButton.Content = _isSidebarCollapsed ? null : "Bookings";
            InventoryButton.Content = _isSidebarCollapsed ? null : "Inventory";
            BillingButton.Content = _isSidebarCollapsed ? null : "Billing";
            ServicesButton.Content = _isSidebarCollapsed ? null : "Services";
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string section = button.Tag?.ToString() ?? "unknown";

                UpdateSelectedButton(button);
                NavigateToSection(section);
            }
        }

        private void NavigateToSection(string section)
        {
            switch (section.ToLower())
            {
                case "rooms":
                    var roomService = new RoomService();
                    var roomViewModel = new RoomViewModel();
                    var roomView = new RoomView();
                    roomView.DataContext = roomViewModel;
                    MainContentArea.Content = roomView;
                    break;

                case "dashboard":
                    MainContentArea.Content = new TextBlock
                    {
                        Text = "Welcome to the Dashboard",
                        FontSize = 24,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    break;

                default:
                    MainContentArea.Content = new TextBlock
                    {
                        Text = $"Section: {section} (coming soon...)",
                        FontSize = 20,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    break;
            }
        }

        private void UpdateSelectedButton(Button selectedButton)
        {
            var buttons = new[] { DashboardButton, GuestsButton, UsersButton, RoomsButton,
                                  BookingsButton, InventoryButton, BillingButton, ServicesButton };

            foreach (var button in buttons)
            {
                button.Background = Brushes.Transparent;
            }

            selectedButton.Background = new SolidColorBrush(Color.FromArgb(100, 74, 144, 226));
        }
    }
}
