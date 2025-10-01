using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class HousekeepingTask
    {

        public int RoomNumber { get; set; }
        public string StaffName { get; set; }
        public DateTime TaskDate { get; set; }
        public string TaskType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
