using STRELA_MED.Data;
using STRELA_MED.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace STRELA_MED.Views
{
    public partial class ExamCardWindow : Window
    {
        private Employee _currentEmployee;
        private AppDbContext _context;
        private MedicalExam _existingExam;

        public ExamCardWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;
            _context = new AppDbContext();

            txtEmployeeName.Text = $"Сотрудник: {employee.LastName} {employee.FirstName} {employee.MiddleName}";
        }

        public ExamCardWindow(MedicalExam exam)
        {
            InitializeComponent();
            _existingExam = exam;
            _currentEmployee = exam.Employee;

            txtEmployeeName.Text = $"Осмотр от {exam.ExamDate:dd.MM.yyyy} — {exam.Employee?.LastName}";
            tbComplaints.Text = exam.Complaints;
            tbBP.Text = exam.BloodPressure;
            tbPulse.Text = exam.Pulse.ToString();
            tbAlcohol.Text = exam.AlcoholLevel.ToString();
            cbExamType.Text = exam.ExamType;
            cbResult.Text = exam.Result;


            if (UserSession.CurrentRole != "Врач" && UserSession.CurrentRole != "Администратор")
            {
                SetReadOnlyMode();
            }
        }

        private void SetReadOnlyMode()
        {
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Content = "Закрыть";

            tbComplaints.IsReadOnly = true;
            tbBP.IsReadOnly = true;
            tbPulse.IsReadOnly = true;
            tbAlcohol.IsReadOnly = true;
            cbExamType.IsEnabled = false;
            cbResult.IsEnabled = false;

            tbComplaints.Background = Brushes.Transparent;
            tbComplaints.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new System.Text.RegularExpressions.Regex("[^0-9,./]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveExam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    DateTime currentExamDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    DateTime nextExamDate = currentExamDate.AddYears(1);

                    var exam = new MedicalExam
                    {
                        EmployeeId = _currentEmployee.Id,
                        ExamType = (cbExamType.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Complaints = tbComplaints.Text,
                        BloodPressure = tbBP.Text,
                        Pulse = int.TryParse(tbPulse.Text, out int p) ? p : 0,
                        AlcoholLevel = double.TryParse(tbAlcohol.Text.Replace('.', ','), out double alc) ? alc : 0.0,
                        Result = (cbResult.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        DoctorName = UserSession.CurrentUserName,

                        ExamDate = currentExamDate,
                        ValidUntil = nextExamDate 
                    };

                    db.MedicalExams.Add(exam);
                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}