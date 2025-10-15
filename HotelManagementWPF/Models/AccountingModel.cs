using System;

namespace HotelManagementWPF.Models
{
    public class TotalSaleDisplay
    {
        public string GuestName { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string TotalPaid { get; set; }
        public string DateCreated { get; set; }

        // New property for filtering
        public DateTime DateObject { get; set; }
    }

    public class InventoryExpenseDisplay
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string PricePerItem { get; set; }
        public string TotalPrice { get; set; }
        public string Restock { get; set; } // static for now
        public decimal TotalPriceValue { get; set; }

        // New property for filtering
        public DateTime RestockDate { get; set; }
    }

    public class LaborExpenseDisplay
    {
        public string StaffName { get; set; }
        public string Department { get; set; }
        public int DutyHours { get; set; }
        public string Rate { get; set; }
        public string Gross { get; set; }
        public string Deductions { get; set; }
        public string NetPay { get; set; }
        public string CreatedAt { get; set; }

        // New property for filtering
        public DateTime CreatedAtObject { get; set; }
    }
}