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
using HotelManagementWPF.Views;
using HotelManagementWPF.ViewModels.Users;

namespace HotelManagementWPF
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarCollapsed = false;
        private bool _isInventoryDropdownExpanded = false;

        private string UserRole = ""; // To store the role of the logged-in user
        private UserViewModel _viewModel; // Your ViewModel instance

        public MainWindow(string role, string fullName)
        {
            InitializeComponent();
            UserRole = role;
            SetModuleVisibility();
            NavigateToSection("dashboard");
            this.WindowState = WindowState.Maximized;
            IWindowService windowService = new WindowService();
            _viewModel = new UserViewModel(windowService);
            DataContext = _viewModel;

            // After ViewModel loads users
            _viewModel.UpdateCurrentUser(fullName, role);

            // Set the user name (example)

        }

        private void SetModuleVisibility()
        {
            // Hide all modules initially
            DashboardButton.Visibility = Visibility.Collapsed;
            BookingsButton.Visibility = Visibility.Collapsed;
            GuestsButton.Visibility = Visibility.Collapsed;
            RoomsButton.Visibility = Visibility.Collapsed;
            UsersButton.Visibility = Visibility.Collapsed;
            EmployeesButton.Visibility = Visibility.Collapsed;

            InventoryButton.Visibility = Visibility.Collapsed;
            ItemsButton.Visibility = Visibility.Collapsed;
            SuppliersButton.Visibility = Visibility.Collapsed;
            ReportsButton.Visibility = Visibility.Collapsed;
            PayrollButton.Visibility = Visibility.Collapsed;
            ServicesButton.Visibility = Visibility.Collapsed;

            // Show modules based on role
            switch (UserRole)
            {
                case "Administrator":
                    // Show all modules
                    DashboardButton.Visibility = Visibility.Visible;
                    BookingsButton.Visibility = Visibility.Visible;
                    GuestsButton.Visibility = Visibility.Visible;
                    RoomsButton.Visibility = Visibility.Visible;
                    UsersButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Visible;
                    InventoryButton.Visibility = Visibility.Visible;
                    ItemsButton.Visibility = Visibility.Visible;
                    SuppliersButton.Visibility = Visibility.Visible;
                    ReportsButton.Visibility = Visibility.Visible;
                    PayrollButton.Visibility = Visibility.Visible;
                    ServicesButton.Visibility = Visibility.Visible;
                    break;

                case "Front Desk":
                    // Show only specific modules
                    BookingsButton.Visibility = Visibility.Visible;
                    GuestsButton.Visibility = Visibility.Visible;
                    RoomsButton.Visibility = Visibility.Visible;
                    break;

                case "Staff Manager":
                    // Show only Housekeeping/Services
                    ServicesButton.Visibility = Visibility.Visible;
                    break;

                case "HR Manager":
                    // Show Payroll and Employees
                    PayrollButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Visible;
                    break;

                case "Inventory Manager":
                    // Show Inventory modules
                    InventoryButton.Visibility = Visibility.Visible;
                    ItemsButton.Visibility = Visibility.Visible;
                    SuppliersButton.Visibility = Visibility.Visible;
                    ReportsButton.Visibility = Visibility.Visible;
                    break;

                default:
                    // Default: show dashboard
                    DashboardButton.Visibility = Visibility.Visible;
                    break;
            }
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

            // Adjust positions and visibility based on collapse state
            if (_isSidebarCollapsed)
            {
                LogoPanel.Margin = new Thickness(5, 0, 0, 0);
                HamburgerButton.Margin = new Thickness(-15, 0, 0, 0);
                HeaderLogoImage.Visibility = Visibility.Collapsed;
                HeaderLogoText.Visibility = Visibility.Collapsed;
                ManagementHeader.Visibility = Visibility.Collapsed;
                OperationsHeader.Visibility = Visibility.Collapsed;

                // Hide text inside buttons
                DashboardText.Visibility = Visibility.Collapsed;
                GuestsText.Visibility = Visibility.Collapsed;
                UsersText.Visibility = Visibility.Collapsed;
                RoomsText.Visibility = Visibility.Collapsed;
                BookingsText.Visibility = Visibility.Collapsed;
                InventoryText.Visibility = Visibility.Collapsed;
                PayrollText.Visibility = Visibility.Collapsed;
                ServicesText.Visibility = Visibility.Collapsed;
                EmployeesText.Visibility = Visibility.Collapsed;

                // Show inventory sub-items directly
                InventoryDropdownContent.Visibility = Visibility.Visible;
                InventoryDropdownContent.Margin = new Thickness(0, 0, 0, 0);
                ItemsText.Visibility = Visibility.Collapsed;
                SuppliersText.Visibility = Visibility.Collapsed;
                ReportsText.Visibility = Visibility.Collapsed;

                // Reset dropdown arrow if expanded
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
                // Expanded sidebar
                HeaderLogoImage.Visibility = Visibility.Visible;
                HeaderLogoText.Visibility = Visibility.Visible;
                ManagementHeader.Visibility = Visibility.Visible;
                OperationsHeader.Visibility = Visibility.Visible;

                // Show button texts
                DashboardText.Visibility = Visibility.Visible;
                GuestsText.Visibility = Visibility.Visible;
                UsersText.Visibility = Visibility.Visible;
                RoomsText.Visibility = Visibility.Visible;
                BookingsText.Visibility = Visibility.Visible;
                InventoryText.Visibility = Visibility.Visible;
                PayrollText.Visibility = Visibility.Visible;
                ServicesText.Visibility = Visibility.Visible;
                EmployeesText.Visibility = Visibility.Visible;

                // Collapse inventory sub-items
                if (!_isInventoryDropdownExpanded)
                {
                    InventoryDropdownContent.Visibility = Visibility.Collapsed;
                }

                InventoryDropdownContent.Margin = new Thickness(20, 0, 0, 0);
            }

            // Hide inventory dropdown arrow when collapsed
            InventoryDropdownButton.Visibility = _isSidebarCollapsed ? Visibility.Collapsed : Visibility.Visible;

            // Adjust icon alignment inside buttons
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
                case "dashboard":
                    var dashboardView = new DashboardView();
                    MainContentArea.Content = dashboardView;
                    HeaderTitle.Text = "Dashboard";
                    break;

                case "rooms":
                    var roomView = new RoomView();
                    MainContentArea.Content = roomView;
                    HeaderTitle.Text = "Room Management";
                    break;

                case "bookings":
                    var bookingView = new HotelManagementWPF.Views.Booking.BookingView();
                    MainContentArea.Content = bookingView;
                    HeaderTitle.Text = "Booking Management";
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
                    HeaderTitle.Text = "House Keeping";
                    break;

                default:
                    HeaderTitle.Text = "Hotel Management System";
                    break;
            }
        }
        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string action = selectedItem.Content.ToString();

                if (action == "Logout")
                {
                    // Confirm logout and navigate
                    var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Open login window
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close(); // Close current window
                    }
                }
                // Reset selection after action
                ActionComboBox.SelectedIndex = -1;
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