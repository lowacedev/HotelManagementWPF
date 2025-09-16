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
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDXXxIeEx0RWFcb1l6dFxMYV9BNQtUQF1hTH5ad0NjXnxedXRXT2RaWkd3");
        }
    }

}
