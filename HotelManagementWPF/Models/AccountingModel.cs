using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class TotalSale
    {
        public string Date { get; set; }
        public string Item { get; set; }
        public string Amount { get; set; }
    }

    public class InventoryExpense
    {
        public string Item { get; set; }
        public int Quantity { get; set; }
        public string Cost { get; set; }
    }

    public class LaborExpense
    {
        public string Employee { get; set; }
        public int Hours { get; set; }
        public string Cost { get; set; }
    }
}
