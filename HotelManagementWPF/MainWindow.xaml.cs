using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels;
using HotelManagementWPF.Views.Room;
using HotelManagementWPF.Views.Dashboard;
using HotelManagementWPF.Views.Guest;
using HotelManagementWPF.Views.Users;
using HotelManagementWPF.Views.Employees;
using HotelManagementWPF.Views.Inventory;
using HotelManagementWPF.Views.Inventory.Items;
using HotelManagementWPF.Views.Inventory.Suppliers;
using HotelManagementWPF.Views.Inventory.Reports;
using HotelManagementWPF.Views.Payroll;
using HotelManagementWPF.Views.Services;


namespace HotelManagementWPF
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarCollapsed = false;
        private bool _isInventoryDropdownExpanded = false;

        public MainWindow()
        {
            InitializeComponent();

            // Load default dashboard at startup
            NavigateToSection("dashboard");
            this.WindowState = WindowState.Maximized;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSidebar();
        }

        private void InventoryDropdown_Click(object sender, RoutedEventArgs e)
        {
            ToggleInventoryDropdown();
        }

        private void ToggleInventoryDropdown()
        {
            _isInventoryDropdownExpanded = !_isInventoryDropdownExpanded;

            InventoryDropdownContent.Visibility = _isInventoryDropdownExpanded 
                ? Visibility.Visible 
                : Visibility.Collapsed;

            // Rotate the arrow
            var rotateTransform = new RotateTransform();
            rotateTransform.Angle = _isInventoryDropdownExpanded ? 180 : 0;
            rotateTransform.CenterX = 6; // Half the width of the arrow
            rotateTransform.CenterY = 4; // Half the height of the arrow
            InventoryDropdownArrow.RenderTransform = rotateTransform;
        }

        private void ToggleSidebar()
        {
            _isSidebarCollapsed = !_isSidebarCollapsed;

            SidebarColumn.Width = _isSidebarCollapsed
                ? new GridLength(60)
                : new GridLength(220);

            // Adjust the hamburger button position and logo panel margin when collapsed
            if (_isSidebarCollapsed)
            {
                // Move hamburger button more to the left and adjust logo panel
                LogoPanel.Margin = new Thickness(5, 0, 0, 0);
                HamburgerButton.Margin = new Thickness(-15, 0, 0, 0);
            }
            else
            {
                // Reset to original positions
                LogoPanel.Margin = new Thickness(0, 0, 0, 0);
                HamburgerButton.Margin = new Thickness(0, 0, 0, 0);
            }

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
            PayrollText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            ServicesText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;
            EmployeesText.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            // When sidebar is collapsed, show inventory sub-items as individual buttons but hide their text
            // When expanded, hide the sub-items (they'll be in the dropdown)
            if (_isSidebarCollapsed)
            {
                // Show the inventory sub-items as separate buttons when collapsed
                InventoryDropdownContent.Visibility = Visibility.Visible;
                InventoryDropdownContent.Margin = new Thickness(0, 0, 0, 0); // Remove left margin when collapsed
                ItemsText.Visibility = Visibility.Collapsed;
                SuppliersText.Visibility = Visibility.Collapsed;
                ReportsText.Visibility = Visibility.Collapsed;
                
                // Reset dropdown state since we're showing items directly
                if (_isInventoryDropdownExpanded)
                {
                    _isInventoryDropdownExpanded = false;
                    var rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 0;
                    rotateTransform.CenterX = 6;
                    rotateTransform.CenterY = 4;
                    InventoryDropdownArrow.RenderTransform = rotateTransform;
                }
            }
            else
            {
                // When expanded, restore the left margin for proper indentation
                InventoryDropdownContent.Margin = new Thickness(20, 0, 0, 0);
                
                // When expanded, hide sub-items (they'll be controlled by dropdown)
                if (!_isInventoryDropdownExpanded)
                {
                    InventoryDropdownContent.Visibility = Visibility.Collapsed;
                }
                ItemsText.Visibility = Visibility.Visible;
                SuppliersText.Visibility = Visibility.Visible;
                ReportsText.Visibility = Visibility.Visible;
            }

            // Hide dropdown arrow when collapsed
            InventoryDropdownButton.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            // Optionally adjust alignment of images when collapsed (keeps them centered)
            var buttons = new[] { DashboardButton, GuestsButton, UsersButton, RoomsButton,
                                  BookingsButton, InventoryButton, PayrollButton, ServicesButton, EmployeesButton,
                                  ItemsButton, SuppliersButton, ReportsButton };

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
                    HeaderTitle.Text = "Room Management";
                    break;

                case "bookings":
                    var bookingView = new HotelManagementWPF.Views.Booking.BookingView();
                    MainContentArea.Content = bookingView;
                    HeaderTitle.Text = "Booking Management";
                    break;

                case "dashboard":
                    var dashboardView = new DashboardView();
                    MainContentArea.Content = dashboardView;
                    HeaderTitle.Text = "Dashboard";
                    break;

                case "guests":
                    var guestView = new GuestView();
                    MainContentArea.Content = guestView;
                    HeaderTitle.Text = "Guest Management";
                    break;

                case "users":
                    var userView = new UserView();
                    MainContentArea.Content = userView;
                    HeaderTitle.Text = "User Management";
                    break;

                case "employees":
                    var employeeView = new EmployeeView();
                    MainContentArea.Content = employeeView;
                    HeaderTitle.Text = "Employee Management";
                    break;

                case "inventory":
                    var inventoryView = new InventoryView();
                    MainContentArea.Content = inventoryView;
                    HeaderTitle.Text = "Inventory Management";
                    break;

                case "items":
                    var itemView = new ItemView();
                    MainContentArea.Content = itemView;
                    HeaderTitle.Text = "Items";
                    break;

                case "suppliers":
                    var supplierView = new SupplierView();
                    MainContentArea.Content = supplierView;
                    HeaderTitle.Text = "Suppliers";
                    break;

                case "reports":
                    var reportView = new ReportView();
                    MainContentArea.Content = reportView;
                    HeaderTitle.Text = "Inventory Reports";
                    break;

                case "payroll":
                    var payrollView = new PayrollView();
                    MainContentArea.Content = payrollView;
        
                    HeaderTitle.Text = "Payroll Management";
                    break;

                case "services":
                    var serviceView = new ServiceView();
                    MainContentArea.Content = serviceView;
                  
                    HeaderTitle.Text = "Services Management";
                    break;

                default:
                    HeaderTitle.Text = "Hotel Management System";
                    break;
            }
        }

        private void UpdateSelectedButton(Button selectedButton)
        {
            var buttons = new[] { DashboardButton, GuestsButton, UsersButton, EmployeesButton, RoomsButton,
                      BookingsButton, InventoryButton, ItemsButton, SuppliersButton, ReportsButton,
                      PayrollButton, ServicesButton };

            foreach (var button in buttons)
            {
                button.Background = Brushes.Transparent;
            }

            selectedButton.Background = new SolidColorBrush(Color.FromArgb(100, 74, 144, 226));
        }
    }
}