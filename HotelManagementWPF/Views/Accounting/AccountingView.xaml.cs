using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Accounting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotelManagementWPF.Views.Accounting
{
    public partial class AccountingView : UserControl
    {
        public AccountingView()
        {
            InitializeComponent();
            var vm = new AccountingViewModel();
            this.DataContext = vm;
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Text == "Search...")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search...";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ExportAll_Click(object sender, RoutedEventArgs e)
        {
            GeneratePdfReport();
        }

        // Export individual tabs
        private void ExportTotalSales_Click(object sender, RoutedEventArgs e)
        {
            GeneratePdfForTotalSales();
        }

        private void ExportInventoryExpenses_Click(object sender, RoutedEventArgs e)
        {
            GeneratePdfForInventoryExpenses();
        }

        private void ExportLaborExpenses_Click(object sender, RoutedEventArgs e)
        {
            GeneratePdfForLaborExpenses();
        }

        private void GeneratePdfReport()
        {
            var viewModel = this.DataContext as AccountingViewModel;
            if (viewModel == null) return;

            // Use the standard Downloads folder path for better cross-user compatibility
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
            string path = downloadsPath + "HotelSummitAccountingReport.pdf";

            // Check if the file exists
            if (File.Exists(path))
            {
                try
                {
                    // Attempt to delete the existing file
                    File.Delete(path);
                }
                catch (IOException)
                {
                    MessageBox.Show("The report file is currently open or locked. Please close it and try again.", "File Locked", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Document doc = new Document(PageSize.A4, 36, 36, 54, 54);
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                // Define Fonts
                var primaryColor = new BaseColor(40, 90, 150);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, primaryColor);
                var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, primaryColor);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summaryValueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor);

                // --- Header Table without Logo ---
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 1f, 3f });
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;
                headerTable.SpacingAfter = 30f;

                // Empty cell for logo space or placeholder
                headerTable.AddCell(new Phrase(""));

                // Add Title and Hotel Name
                PdfPCell titleCell = new PdfPCell();
                titleCell.Border = Rectangle.NO_BORDER;
                titleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                // Hotel Name
                Paragraph hotelName = new Paragraph("Hotel Summit", subTitleFont)
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                titleCell.AddElement(hotelName);

                // Report Title
                Paragraph title = new Paragraph("Accounting Report", titleFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 5
                };
                titleCell.AddElement(title);

                // Date
                Paragraph date = new Paragraph($"Generated On: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont)
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                titleCell.AddElement(date);

                headerTable.AddCell(titleCell);
                doc.Add(headerTable);

                // --- Totals Summary ---
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 2.5f, 2.5f, 2.5f, 2.5f });
                summaryTable.SpacingBefore = 10f;
                summaryTable.SpacingAfter = 20f;

                // Use Rectangle.BOTTOM constant for bottom border
                summaryTable.DefaultCell.Border = 2; // Rectangle.BOTTOM = 2
                summaryTable.DefaultCell.BorderColor = primaryColor;
                summaryTable.DefaultCell.BorderWidthBottom = 2f;
                summaryTable.DefaultCell.PaddingBottom = 10f;
                summaryTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                // Labels
                summaryTable.AddCell(new Phrase("Total Revenue", boldFont));
                summaryTable.AddCell(new Phrase("Total Sales", boldFont));
                summaryTable.AddCell(new Phrase("Inventory Expenses", boldFont));
                summaryTable.AddCell(new Phrase("Labor Expenses", boldFont));

                // Values
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalRevenue:N2}", summaryValueFont));
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalSalesAmount:N2}", summaryValueFont));
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalInventoryExpenses:N2}", summaryValueFont));
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalLaborExpenses:N2}", summaryValueFont));

                doc.Add(summaryTable);

                // Helper method for table headers
                Action<PdfPTable, string, Font, BaseColor> AddHeaderCell = (table, text, font, bgColor) =>
                {
                    PdfPCell cell = new PdfPCell(new Phrase(text, font));
                    cell.BackgroundColor = bgColor;
                    cell.Padding = 5f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                };

                // --- Total Sales Data Table ---
                doc.Add(new Paragraph("Sales Transactions", subTitleFont) { SpacingBefore = 15f, SpacingAfter = 10f });
                PdfPTable salesTable = new PdfPTable(5);
                salesTable.WidthPercentage = 100;
                salesTable.SetWidths(new float[] { 2.5f, 1.5f, 2f, 2f, 2f });

                AddHeaderCell(salesTable, "Guest Name", headerFont, primaryColor);
                AddHeaderCell(salesTable, "Room No.", headerFont, primaryColor);
                AddHeaderCell(salesTable, "Room Type", headerFont, primaryColor);
                AddHeaderCell(salesTable, "Total Paid", headerFont, primaryColor);
                AddHeaderCell(salesTable, "Date Created", headerFont, primaryColor);

                foreach (var item in viewModel.TotalSales)
                {
                    salesTable.AddCell(new Phrase(item.GuestName, normalFont));
                    salesTable.AddCell(new Phrase(item.RoomNumber, normalFont));
                    salesTable.AddCell(new Phrase(item.RoomType, normalFont));
                    salesTable.AddCell(new Phrase(item.TotalPaid, normalFont));
                    salesTable.AddCell(new Phrase(item.DateCreated, normalFont));
                }
                doc.Add(salesTable);

                // --- Inventory Expenses Data Table ---
                doc.Add(new Paragraph("Inventory Expenses", subTitleFont) { SpacingBefore = 20f, SpacingAfter = 10f });
                PdfPTable inventoryTable = new PdfPTable(5);
                inventoryTable.WidthPercentage = 100;
                inventoryTable.SetWidths(new float[] { 3f, 1f, 2f, 2f, 2f });

                AddHeaderCell(inventoryTable, "Item Name", headerFont, primaryColor);
                AddHeaderCell(inventoryTable, "Quantity", headerFont, primaryColor);
                AddHeaderCell(inventoryTable, "Price per Item", headerFont, primaryColor);
                AddHeaderCell(inventoryTable, "Total Price", headerFont, primaryColor);
                AddHeaderCell(inventoryTable, "Restock Date", headerFont, primaryColor);

                foreach (var item in viewModel.InventoryExpenses)
                {
                    inventoryTable.AddCell(new Phrase(item.ItemName, normalFont));
                    inventoryTable.AddCell(new Phrase(item.Quantity.ToString(), normalFont));
                    inventoryTable.AddCell(new Phrase(item.PricePerItem, normalFont));
                    inventoryTable.AddCell(new Phrase(item.TotalPrice, normalFont));
                    inventoryTable.AddCell(new Phrase(item.Restock, normalFont));
                }
                doc.Add(inventoryTable);

                // --- Labor Expenses Data Table ---
                doc.Add(new Paragraph("Labor Expenses", subTitleFont) { SpacingBefore = 20f, SpacingAfter = 10f });
                PdfPTable laborTable = new PdfPTable(8);
                laborTable.WidthPercentage = 100;
                laborTable.SetWidths(new float[] { 2f, 1.5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f });

                AddHeaderCell(laborTable, "Staff Name", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Department", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Hours", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Rate", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Gross", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Deductions", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Net Pay", headerFont, primaryColor);
                AddHeaderCell(laborTable, "Date Created", headerFont, primaryColor);

                foreach (var item in viewModel.LaborExpenses)
                {
                    laborTable.AddCell(new Phrase(item.StaffName, normalFont));
                    laborTable.AddCell(new Phrase(item.Department, normalFont));
                    laborTable.AddCell(new Phrase(item.DutyHours.ToString(), normalFont));
                    laborTable.AddCell(new Phrase(item.Rate, normalFont));
                    laborTable.AddCell(new Phrase(item.Gross, normalFont));
                    laborTable.AddCell(new Phrase(item.Deductions, normalFont));
                    laborTable.AddCell(new Phrase(item.NetPay, normalFont));
                    laborTable.AddCell(new Phrase(item.CreatedAt, normalFont));
                }
                doc.Add(laborTable);

                MessageBox.Show($"PDF Exported successfully to:\n{path}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating PDF: " + ex.Message + "\n\nMake sure the file is not currently open and the logo exists at Resources/Icons/logo.png.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (doc.IsOpen())
                {
                    doc.Close();
                }
            }
        }

        private void GeneratePdfForTotalSales()
        {
            var viewModel = this.DataContext as AccountingViewModel;
            if (viewModel == null) return;

            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
            string path = downloadsPath + "TotalSalesReport.pdf";

            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { MessageBox.Show("File is open/locked."); return; }
            }

            Document doc = new Document(PageSize.A4, 36, 36, 54, 54);
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                // Add header
                AddReportHeader(doc, "Accounting");

                var primaryColor = new BaseColor(40, 90, 150);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, primaryColor);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summaryValueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f);

                Paragraph title = new Paragraph("Total Sales Report", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 };
                doc.Add(title);
                // -- Only Total Revenue and Total Sales in summary --
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 2.5f, 2.5f, 2.5f, 2.5f });
                summaryTable.SpacingBefore = 10f;
                summaryTable.SpacingAfter = 20f;

                // Style borders
                summaryTable.DefaultCell.Border = 2; // Rectangle.BOTTOM
                summaryTable.DefaultCell.BorderColor = primaryColor;
                summaryTable.DefaultCell.BorderWidthBottom = 2f;
                summaryTable.DefaultCell.PaddingBottom = 10f;
                summaryTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                // Labels
                summaryTable.AddCell(new Phrase("Total Sales:", boldFont));

                // Values
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalSalesAmount:N2}", summaryValueFont));
                // Empty cells for the other two
                summaryTable.AddCell(new Phrase(""));
                summaryTable.AddCell(new Phrase(""));

                // Add summary table to document
                doc.Add(summaryTable);

                // Now add the specific data table for Total Sales
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2.5f, 1.5f, 2f, 2f, 2f });
                string[] headers = { "Guest Name", "Room No.", "Room Type", "Total Paid", "Date Created" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = primaryColor,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                foreach (var item in viewModel.TotalSales)
                {
                    table.AddCell(new Phrase(item.GuestName, normalFont));
                    table.AddCell(new Phrase(item.RoomNumber, normalFont));
                    table.AddCell(new Phrase(item.RoomType, normalFont));
                    table.AddCell(new Phrase(item.TotalPaid, normalFont));
                    table.AddCell(new Phrase(item.DateCreated, normalFont));
                }

                doc.Add(table);
                MessageBox.Show($"Total Sales PDF exported to:\n{path}", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (doc.IsOpen()) doc.Close();
            }
        }

        private void GeneratePdfForInventoryExpenses()
        {
            var viewModel = this.DataContext as AccountingViewModel;
            if (viewModel == null) return;

            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
            string path = downloadsPath + "InventoryExpensesReport.pdf";

            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { MessageBox.Show("File is open/locked."); return; }
            }

            Document doc = new Document(PageSize.A4, 36, 36, 54, 54);
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();
                AddReportHeader(doc, "Accounting");

                var primaryColor = new BaseColor(40, 90, 150);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, primaryColor);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summaryValueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f);
                // Title
                Paragraph title = new Paragraph("Inventory Expenses Report", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 };
                doc.Add(title);

                // -- Only Total Revenue and Total Sales in summary --
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 2.5f, 2.5f, 2.5f, 2.5f });
                summaryTable.SpacingBefore = 10f;
                summaryTable.SpacingAfter = 20f;

                // Style borders
                summaryTable.DefaultCell.Border = 2; // Rectangle.BOTTOM
                summaryTable.DefaultCell.BorderColor = primaryColor;
                summaryTable.DefaultCell.BorderWidthBottom = 2f;
                summaryTable.DefaultCell.PaddingBottom = 10f;
                summaryTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                // Labels
                summaryTable.AddCell(new Phrase("Inventory Expenses:", boldFont));

                // Values
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalInventoryExpenses:N2}", summaryValueFont));
                // Empty cells for the other two
                summaryTable.AddCell(new Phrase(""));
                summaryTable.AddCell(new Phrase(""));

                // Add summary table to document
                doc.Add(summaryTable);

                // Table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3f, 1f, 2f, 2f, 2f });

                string[] headers = { "Item Name", "Quantity", "Price per Item", "Total Price", "Restock Date" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = primaryColor,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                foreach (var item in viewModel.InventoryExpenses)
                {
                    table.AddCell(new Phrase(item.ItemName, normalFont));
                    table.AddCell(new Phrase(item.Quantity.ToString(), normalFont));
                    table.AddCell(new Phrase(item.PricePerItem, normalFont));
                    table.AddCell(new Phrase(item.TotalPrice, normalFont));
                    table.AddCell(new Phrase(item.Restock, normalFont));
                }

                doc.Add(table);
                MessageBox.Show($"Inventory Expenses PDF exported to:\n{path}", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (doc.IsOpen()) doc.Close();
            }
        }

        private void GeneratePdfForLaborExpenses()
        {
            var viewModel = this.DataContext as AccountingViewModel;
            if (viewModel == null) return;

            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
            string path = downloadsPath + "LaborExpensesReport.pdf";

            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { MessageBox.Show("File is open/locked."); return; }
            }

            Document doc = new Document(PageSize.A4, 36, 36, 54, 54);
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();
                AddReportHeader(doc, "Accounting");

                var primaryColor = new BaseColor(40, 90, 150);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, primaryColor);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summaryValueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f);
                // Title
                Paragraph title = new Paragraph("Labor Expenses Report", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 };
                doc.Add(title);

                // -- Only Total Revenue and Total Sales in summary --
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 2.5f, 2.5f, 2.5f, 2.5f });
                summaryTable.SpacingBefore = 10f;
                summaryTable.SpacingAfter = 20f;

                // Style borders
                summaryTable.DefaultCell.Border = 2; // Rectangle.BOTTOM
                summaryTable.DefaultCell.BorderColor = primaryColor;
                summaryTable.DefaultCell.BorderWidthBottom = 2f;
                summaryTable.DefaultCell.PaddingBottom = 10f;
                summaryTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                // Labels
                summaryTable.AddCell(new Phrase("Labor Expenses:", boldFont));

                // Values
                summaryTable.AddCell(new Phrase($"₱{viewModel.TotalLaborExpenses:N2}", summaryValueFont));
                // Empty cells for the other two
                summaryTable.AddCell(new Phrase(""));
                summaryTable.AddCell(new Phrase(""));

                // Add summary table to document
                doc.Add(summaryTable);


                // Table
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 1.5f, 1f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f });

                string[] headers = { "Staff Name", "Department", "Hours", "Rate", "Gross", "Deductions", "Net Pay", "Date Created" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = primaryColor,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                foreach (var item in viewModel.LaborExpenses)
                {
                    table.AddCell(new Phrase(item.StaffName, normalFont));
                    table.AddCell(new Phrase(item.Department, normalFont));
                    table.AddCell(new Phrase(item.DutyHours.ToString(), normalFont));
                    table.AddCell(new Phrase(item.Rate, normalFont));
                    table.AddCell(new Phrase(item.Gross, normalFont));
                    table.AddCell(new Phrase(item.Deductions, normalFont));
                    table.AddCell(new Phrase(item.NetPay, normalFont));
                    table.AddCell(new Phrase(item.CreatedAt, normalFont));
                }

                doc.Add(table);
                MessageBox.Show($"Labor Expenses PDF exported to:\n{path}", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (doc.IsOpen()) doc.Close();
            }
        }

        private void AddReportHeader(Document doc, string reportTitle)
        {
            var primaryColor = new BaseColor(40, 90, 150);
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, primaryColor);
            var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, primaryColor);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // Create a table with 2 columns for layout
            PdfPTable headerTable = new PdfPTable(2);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 1f, 3f });
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;
            headerTable.SpacingAfter = 20f;

            // Empty cell for logo space or placeholder
            headerTable.AddCell(new Phrase(""));

            // Add hotel name, report title, and date
            PdfPCell titleCell = new PdfPCell();
            titleCell.Border = Rectangle.NO_BORDER;
            titleCell.HorizontalAlignment = Element.ALIGN_RIGHT;

            // Hotel Name
            Paragraph hotelName = new Paragraph("Hotel Summit", subTitleFont);
            hotelName.Alignment = Element.ALIGN_RIGHT;
            titleCell.AddElement(hotelName);

            // Report Title
            Paragraph title = new Paragraph(reportTitle, titleFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 5
            };
            titleCell.AddElement(title);

            // Date
            Paragraph date = new Paragraph($"Generated On: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont)
            {
                Alignment = Element.ALIGN_RIGHT
            };
            titleCell.AddElement(date);

            headerTable.AddCell(titleCell);
            doc.Add(headerTable);
        }
        private void FilterDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = this.DataContext as AccountingViewModel;
            if (viewModel != null)
            {
                viewModel.FilterMonth = (sender as DatePicker)?.SelectedDate;
                viewModel.ApplyFilter();
            }
        }
    }

}