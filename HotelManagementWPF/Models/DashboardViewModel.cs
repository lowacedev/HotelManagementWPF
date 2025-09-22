using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class DashboardViewModel
    {
        public ChartValues<double> MonthlyOccupancyValues { get; set; } = new ChartValues<double>
    {
        75, 80, 85, 90, 88, 92, 95, 93, 89, 87, 90, 94
    };

        public List<string> MonthLabels { get; set; } = new List<string>
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

        public Func<double, string> PercentageFormatter => value => value + "%";
    }

}
