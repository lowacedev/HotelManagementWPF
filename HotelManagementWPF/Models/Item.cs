using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public int SupplierId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string StockLevel { get; set; } = string.Empty;

        // Navigation property for display
        public string Name { get; set; } = string.Empty;
        public int RestockQuantity { get; set; } // Add this property

    }

}