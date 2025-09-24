using System.Configuration;
using System.Data;
using System.Windows;
using Syncfusion.Licensing;

namespace HotelManagementWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Register Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDXXxIeEx0RWFcb1l6dFxMYV9BNQtUQF1hTH5ad0NjXnxedXRXT2RaWkd3");
        }

        /// <summary>
        /// This method is called when the application starts up.
        /// You can perform initial setup here, like showing the main window.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // You can create and show your main window here.
            // For example, if your main window is named "MainWindow".
            // MainWindow mainWindow = new MainWindow();
            // mainWindow.Show();
        }
    }
}