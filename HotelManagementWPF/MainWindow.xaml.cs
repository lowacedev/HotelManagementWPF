using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels;
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

            // Hide logo image and logo text when collapsed
            HeaderLogoImage.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            HeaderLogoText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            ManagementHeader.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            OperationsHeader.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            // Hide or show the label text inside each button so the icons/images remain visible when collapsed
            DashboardText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            GuestsText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            UsersText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            RoomsText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            BookingsText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            InventoryText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            BillingText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            ServicesText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            // Optionally adjust alignment of images when collapsed (keeps them centered)
            var buttons = new[] { DashboardButton, GuestsButton, UsersButton, RoomsButton,
                                  BookingsButton, InventoryButton, BillingButton, ServicesButton };

            foreach (var button in buttons)
            {
                button.HorizontalContentAlignment = _isSidebarCollapsed ? HorizontalAlignment.Center : HorizontalAlignment.Left;
            }
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
                    var windowService = new WindowService(); 
                    var roomViewModel = new RoomViewModel(windowService);
                    var roomView = new RoomView();
                    roomView.DataContext = roomViewModel;
                    MainContentArea.Content = roomView;
                    break;

                case "bookings":
                    var bookingView = new HotelManagementWPF.Views.Booking.BookingView();
                    MainContentArea.Content = bookingView;
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
