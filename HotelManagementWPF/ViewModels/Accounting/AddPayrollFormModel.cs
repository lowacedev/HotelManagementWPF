using DatabaseProject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagementWPF.ViewModels.Accounting
{
    class AddPayrollFormModel
    {
        public readonly DbConnections _db;

        public AddPayrollFormModel()
        {
            _db = new DbConnections();
        }

        public async Task AddPayrollAsync(string employeeName, string department, int dutyHours, decimal rate)
        {
            try
            {
                // Step 1: Get staff_id based on Employee Name
                string getStaffIdQuery = "SELECT staff_id FROM tbl_Staff WHERE name = @Name AND department = @Department";

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", employeeName },
                    { "@Department", department }
                };

                var dt = await _db.readDataWithParametersAsync(getStaffIdQuery, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Employee not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int staffId = Convert.ToInt32(dt.Rows[0]["staff_id"]);

                // Step 2: Insert into tbl_Payroll
                string insertPayrollSql = @"
                    INSERT INTO tbl_Payroll (staff_id, duty_hours, rate, total_gross_pay, deductions, total_net_pay, created_at)
                    VALUES (@StaffId, @DutyHours, @Rate, 0, 0, 0, @CreatedAt)";

                var insertParams = new Dictionary<string, object>
                {
                    { "@StaffId", staffId },
                    { "@DutyHours", dutyHours },
                    { "@Rate", rate },
                    { "@CreatedAt", DateTime.Now.Date }
                };

                await _db.ExecuteNonQueryAsync(insertPayrollSql, insertParams);

                MessageBox.Show("Payroll record added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding payroll: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}