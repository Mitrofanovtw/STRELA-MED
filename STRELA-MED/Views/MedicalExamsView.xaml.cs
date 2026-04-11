using Microsoft.EntityFrameworkCore;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace STRELA_MED.Views
{
    public partial class MedicalExamsView : UserControl
    {
        public MedicalExamsView()
        {
            InitializeComponent();
            LoadExams();
            ApplyPermissions();
        }

        private void LoadExams(string searchText = "")
        {
            using (var db = new AppDbContext())
            {
                var query = db.MedicalExams
                    .Include(e => e.Employee)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string search = searchText.ToLower();
                    query = query.Where(e =>
                        e.Employee.LastName.ToLower().Contains(search) ||
                        e.Employee.FirstName.ToLower().Contains(search) ||
                        e.ExamType.ToLower().Contains(search));
                }

                var results = query.ToList();
                ExamsGrid.ItemsSource = results;

                if (results.Any())
                {
                    ExamsGrid.Visibility = Visibility.Visible;
                    EmptyStateText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ExamsGrid.Visibility = Visibility.Collapsed;
                    EmptyStateText.Visibility = Visibility.Visible;
                    EmptyStateText.Text = string.IsNullOrEmpty(searchText)
                        ? "База медосмотров пуста"
                        : "По вашему запросу ничего не найдено";
                }
            }
        }

        private void ApplyPermissions()
        {
            var user = CurrentUser.Data;
            if (user == null) return;

            if (user.Role != "Doctor")
            {
                BtnCallToExam.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchEmployeeExams_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadExams(SearchEmployeeExams.Text);
        }

        private void SearchExams_Click(object sender, RoutedEventArgs e)
        {
            LoadExams(SearchEmployeeExams.Text);
        }

        private void CallToExam_Click(object sender, RoutedEventArgs e)
        {
            if (ExamsGrid.SelectedItem is MedicalExam selected)
            {
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
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись в таблице!");
            }
        }
    }
}