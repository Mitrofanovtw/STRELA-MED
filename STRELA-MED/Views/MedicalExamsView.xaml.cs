using Microsoft.EntityFrameworkCore;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                        e.Employee.FirstName.ToLower().Contains(search));
                }

                var results = query.ToList();

                if (results.Any())
                {
                    ExamsGrid.ItemsSource = results;
                    ExamsGrid.Visibility = Visibility.Visible;
                    EmptyStateText.Visibility = Visibility.Collapsed;
                }
                else if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var employeeSearch = db.Employees
                        .Where(emp => emp.LastName.ToLower().Contains(searchText.ToLower()))
                        .ToList();

                    if (employeeSearch.Any())
                    {
                        ExamsGrid.Visibility = Visibility.Collapsed;
                        EmptyStateText.Visibility = Visibility.Visible;
                        EmptyStateText.Text = $"У сотрудника {employeeSearch[0].LastName} еще нет записей об осмотрах.\nНажмите 'Вызвать', чтобы отправить уведомление.";

                        BtnCallToExam.Tag = employeeSearch[0];
                    }
                    else
                    {
                        ExamsGrid.Visibility = Visibility.Collapsed;
                        EmptyStateText.Visibility = Visibility.Visible;
                        EmptyStateText.Text = "Сотрудник не найден в базе данных";
                    }
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
                BtnNewExam.Visibility = Visibility.Collapsed;
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
            var selectionWindow = new CallEmployeeSelectionWindow();
            selectionWindow.Owner = Window.GetWindow(this);

            if (selectionWindow.ShowDialog() == true)
            {
                Employee targetEmployee = selectionWindow.SelectedEmployee;

                string defaultText = $"Уважаемый(ая) {targetEmployee.FirstName} {targetEmployee.MiddleName}, приглашаем вас на плановый медосмотр.";
                var sendWindow = new SendNotificationWindow(defaultText);

                if (sendWindow.ShowDialog() == true)
                {
                    using (var db = new AppDbContext())
                    {
                        var notification = new Notification
                        {
                            EmployeeId = targetEmployee.Id,
                            Message = sendWindow.FinalMessage,
                            AppointmentDate = DateTime.SpecifyKind(DateTime.Now.AddDays(1), DateTimeKind.Utc),
                            IsRead = false
                        };
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                        MessageBox.Show("Уведомление успешно отправлено!");
                    }
                }
            }
        }

        private void ExamsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedExam = ExamsGrid.SelectedItem as MedicalExam;
            if (selectedExam != null)
            {
                var viewWindow = new ExamCardWindow(selectedExam);
                viewWindow.Owner = Window.GetWindow(this);
                viewWindow.ShowDialog();
            }
        }

        private void DeleteExam_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exam = button?.DataContext as MedicalExam;

            if (exam == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись о медосмотре сотрудника {exam.Employee?.LastName} от {exam.ExamDate:dd.MM.yyyy}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        db.MedicalExams.Remove(exam);
                        db.SaveChanges();
                    }

                    LoadExams(SearchEmployeeExams.Text);

                    MessageBox.Show("Запись успешно удалена.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NewExam_Click(object sender, RoutedEventArgs e)
        {
            var selectWindow = new EmployeeSelectionWindow();
            selectWindow.Owner = Window.GetWindow(this);

            if (selectWindow.ShowDialog() == true)
            {
                var targetEmployee = selectWindow.SelectedEmployee;
                var examCard = new ExamCardWindow(targetEmployee);

                if (examCard.ShowDialog() == true)
                {
                    LoadExams();
                }
            }
        }
    }
}