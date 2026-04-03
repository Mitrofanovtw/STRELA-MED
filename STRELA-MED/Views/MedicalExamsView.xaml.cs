using Microsoft.EntityFrameworkCore;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для MedicalExamsView.xaml
    /// </summary>
    public partial class MedicalExamsView : UserControl
    {
        public MedicalExamsView()
        {
            InitializeComponent();
        }

        private void SearchExams_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchEmployeeExams.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) return;

            using (var db = new AppDbContext())
            {
                var results = db.MedicalExams
                    .Include(ex => ex.Employee)
                    .Where(ex => ex.Employee.LastName.ToLower().StartsWith(searchText))
                    .ToList();

                if (results.Any())
                {
                    ExamsGrid.ItemsSource = results;
                    ExamsGrid.Visibility = Visibility.Visible;
                    EmptyStateText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ExamsGrid.Visibility = Visibility.Collapsed;
                    EmptyStateText.Visibility = Visibility.Visible;
                    EmptyStateText.Text = "Записей не найдено.";
                }
            }
        }

        private void CallToExam_Click(object sender, RoutedEventArgs e)
        {
            var selected = ExamsGrid.SelectedItem as MedicalExam;
            if (selected == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника из списка!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var notification = new Notification
                {
                    EmployeeId = selected.EmployeeId,
                    Message = $"Доброго времени суток, {selected.Employee.LastName}, вам необходимо явиться в медпункт для прохождения медицинского осмотра.",
                    AppointmentDate = DateTime.SpecifyKind(DateTime.Now.AddDays(1), DateTimeKind.Utc),
                    IsRead = false
                };

                db.Notifications.Add(notification);
                db.SaveChanges();

                MessageBox.Show($"Уведомление для {selected.Employee.LastName} успешно отправлено!");
            }
        }
    }
}
