using HotelManagementWPF.ViewModels.Accounting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementWPF.Views.Accounting
{
    public partial class AddPayrollForm : Window
    {
        private readonly AddPayrollFormModel _model;
        private List<StaffMember> StaffList = new List<StaffMember>();

        public AddPayrollForm()
        {
            InitializeComponent();
            _model = new AddPayrollFormModel();

            LoadEmployeesAsync();

            // Set default selection if available
            Loaded += (s, e) =>
            {
                if (EmployeeComboBox.Items.Count > 0)
                {
                    EmployeeComboBox.SelectedIndex = 0;
                }
            };
        }

        private async void LoadEmployeesAsync()
        {
            await FetchEmployeesAsync();

            EmployeeComboBox.ItemsSource = StaffList;
            EmployeeComboBox.DisplayMemberPath = "Name";
            EmployeeComboBox.SelectedValuePath = "StaffId";

            if (StaffList.Count > 0)
            {
                EmployeeComboBox.SelectedIndex = 0;
            }
        }

        private async Task FetchEmployeesAsync()
        {
            try
            {
                var sql = "SELECT staff_id, name, department FROM tbl_Staff";
                var dt = await _model._db.readDataWithParametersAsync(sql, new Dictionary<string, object>());

                StaffList.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    StaffList.Add(new StaffMember
                    {
                        StaffId = Convert.ToInt32(row["staff_id"]),
                        Name = row["name"].ToString(),
                        Department = row["department"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem is StaffMember selectedStaff)
            {
                DepartmentTextBox.Text = selectedStaff.Department;
            }
        }

        private async void AddPayrollButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem is StaffMember selectedStaff)
            {
                string employeeName = selectedStaff.Name;
                string department = selectedStaff.Department;

                if (string.IsNullOrEmpty(employeeName) || string.IsNullOrEmpty(department))
                {
                    MessageBox.Show("Please fill all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(SalaryTextBox.Text, out int dutyHours))
                {
                    MessageBox.Show("Duty Hours must be a number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(RateTextBox.Text, out decimal rate))
                {
                    MessageBox.Show("Rate must be a number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _model.AddPayrollAsync(employeeName, department, dutyHours, rate);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an employee.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private class StaffMember
        {
            public int StaffId { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
        }
    }
}