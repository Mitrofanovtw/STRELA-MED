using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using Microsoft.Win32;
using STRELA_MED.Data;
using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace STRELA_MED.Views
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
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