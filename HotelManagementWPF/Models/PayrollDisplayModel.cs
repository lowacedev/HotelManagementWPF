using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class PayrollDisplayModel
    {
        public int PayrollId { get; set; }
        public string StaffName { get; set; }
        public string Department { get; set; }
        public int DutyHours { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalGrossPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal TotalNetPay { get; set; }
        public string CreatedAt { get; set; }
    }
}
