using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace STRELA_MED.Views
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private void ShowEmployees_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dg = new DataGrid
            {
                Margin = new Thickness(15),
                AutoGenerateColumns = false,
                IsReadOnly = true,
                ItemsSource = GetAllEmployees(),
                Columns =
        {
            new DataGridTextColumn { Header = "Фамилия", Binding = new Binding("LastName"), Width = 150 },
            new DataGridTextColumn { Header = "Имя", Binding = new Binding("FirstName"), Width = 150 },
            new DataGridTextColumn { Header = "Должность", Binding = new Binding("Position"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) }
        }
            };

            dg.MouseDoubleClick += (s, ev) =>
            {
                if (dg.SelectedItem is Employee selectedEmp)
                {
                    var analytics = new EmployeeAnalyticsWindow(selectedEmp);
                    analytics.ShowDialog();
                }
            };

            var employeesWindow = new Window
            {
                Title = "Реестр сотрудников (двойной клик для аналитики)",
                Width = 800,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = dg
            };
            employeesWindow.ShowDialog();
        }

        private void ShowPending_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pendingWindow = new Window
            {
                Title = "Список сотрудников, ожидающих осмотра",
                Width = 800,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new DataGrid
                {
                    Margin = new Thickness(15),
                    AutoGenerateColumns = false,
                    IsReadOnly = true,
                    ItemsSource = GetPendingEmployees(),
                    Columns =
                    {
                        new DataGridTextColumn { Header = "Фамилия", Binding = new Binding("LastName"), Width = 200 },
                        new DataGridTextColumn { Header = "Должность", Binding = new Binding("Position"), Width = 200 },
                        new DataGridTextColumn { Header = "Статус", Binding = new Binding("Status"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) }
                    }
                }
            };
            pendingWindow.ShowDialog();
        }

        private void ShowLowStock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var lowStockList = new Window
            {
                Title = "Ведомость дефицита товаров",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new DataGrid
                {
                    Margin = new Thickness(10),
                    AutoGenerateColumns = false,
                    IsReadOnly = true,
                    ItemsSource = GetLowStockData(),
                    Columns =
                    {
                        new DataGridTextColumn { Header = "Наименование", Binding = new Binding("Name"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) },
                        new DataGridTextColumn { Header = "Остаток", Binding = new Binding("Quantity"), Width = 100 }
                    }
                }
            };
            lowStockList.ShowDialog();
        }
        private System.Collections.IEnumerable GetAllEmployees()
        {
            using (var db = new AppDbContext())
            {
                return db.Employees.OrderBy(e => e.LastName).ToList();
            }
        }

        private System.Collections.IEnumerable GetPendingEmployees()
        {
            using (var db = new AppDbContext())
            {
                var today = DateTime.UtcNow;
                return db.Employees.ToList()
                    .Where(e => !db.MedicalExams.Any(m => m.EmployeeId == e.Id) ||
                                db.MedicalExams.Where(m => m.EmployeeId == e.Id).Max(m => m.ValidUntil) < today)
                    .Select(e => new { e.LastName, e.FirstName, e.Position, Status = "Требуется осмотр" })
                    .ToList();
            }
        }

        private System.Collections.IEnumerable GetLowStockData()
        {
            using (var db = new AppDbContext())
            {
                return db.Medicines.Where(m => m.Quantity < 10).ToList();
            }
        }

        private void GeneratePdfReport_Click(object sender, RoutedEventArgs e)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Отчет_СтрелаМед_{DateTime.Now:dd_MM_yyyy_HH_mm}"
            };

            if (sfd.ShowDialog() != true) return;

            try
            {
                if (File.Exists(sfd.FileName))
                {
                    try { using (FileStream fs = File.OpenWrite(sfd.FileName)) { } }
                    catch { MessageBox.Show("Файл уже открыт в другой программе! Закройте его и повторите."); return; }
                }

                using (PdfWriter writer = new PdfWriter(sfd.FileName))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    PdfFont font;
                    PdfFont boldFont;
                    try
                    {
                        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                        font = PdfFontFactory.CreateFont(Path.Combine(windir, "arial.ttf"), "Identity-H");
                        boldFont = PdfFontFactory.CreateFont(Path.Combine(windir, "arialbd.ttf"), "Identity-H");
                    }
                    catch
                    {
                        font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, "Cp1251");
                        boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD, "Cp1251");
                    }

                    document.SetFont(font);

                    document.Add(new Paragraph("АНАЛИТИЧЕСКИЙ ОТЧЕТ: ПО СТРЕЛА-МЕД")
                        .SetFontSize(18).SetFont(boldFont)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    using (var db = new AppDbContext())
                    {
                        document.Add(new Paragraph("\n1. СОСТОЯНИЕ СКЛАДА (ДЕФИЦИТ)").SetFontSize(14).SetFont(boldFont));

                        var lowStock = db.Medicines.Where(m => m.Quantity < 10).ToList();
                        Table stockTable = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).SetWidth(UnitValue.CreatePercentValue(100));

                        stockTable.AddHeaderCell(new Cell().Add(new Paragraph("Наименование").SetFont(boldFont)));
                        stockTable.AddHeaderCell(new Cell().Add(new Paragraph("Остаток").SetFont(boldFont)));

                        foreach (var item in lowStock)
                        {
                            stockTable.AddCell(new Paragraph(item.Name ?? "---"));
                            stockTable.AddCell(new Paragraph(item.Quantity.ToString()));
                        }
                        document.Add(stockTable);

                        document.Add(new Paragraph("\n2. ОБЩАЯ СТАТИСТИКА").SetFontSize(14).SetFont(boldFont));
                        document.Add(new Paragraph($"Всего сотрудников: {db.Employees.Count()}"));
                        document.Add(new Paragraph($"Всего медосмотров в базе: {db.MedicalExams.Count()}"));
                    }

                    document.Close();
                }
                MessageBox.Show("Отчет успешно создан!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка iText: {ex.Message}\n\nДетали: {ex.StackTrace}", "Ошибка генерации");
            }
        }
    }
}