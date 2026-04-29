using System;
using System.Windows;
using STRELA_MED.Data;
using STRELA_MED.Models;

namespace STRELA_MED.Views
{
    public partial class ExamCardWindow : Window
    {
        private Employee _currentEmployee;
        private AppDbContext _context;

        public ExamCardWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;
            _context = new AppDbContext();

            txtEmployeeName.Text = $"Сотрудник: {employee.LastName} {employee.FirstName} {employee.MiddleName}";
        }

        private void SaveExam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exam = new MedicalExam
                {
                    EmployeeId = _currentEmployee.Id,
                    ExamDate = DateTime.UtcNow,
                    ExamType = tbExamType.Text,
                    DoctorName = UserSession.CurrentUserName,
                    Result = cbResult.Text,
                    ValidUntil = DateTime.UtcNow.AddMonths(6),
                    BloodPressure = tbBP.Text,
                    Pulse = int.TryParse(tbPulse.Text, out int p) ? p : 0
                };

                _context.MedicalExams.Add(exam);
                _context.SaveChanges();

                MessageBox.Show("Данные медосмотра успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Ошибка при сохранении: {message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}