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
    /// Логика взаимодействия для CallEmployeeSelectionWindow.xaml
    /// </summary>
    public partial class CallEmployeeSelectionWindow : Window
    {
        public Employee SelectedEmployee { get; private set; }

        public CallEmployeeSelectionWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var employees = db.Employees.ToList();
                var exams = db.MedicalExams.ToList();

                var displayList = employees.Select(emp => new EmployeeExamStatus
                {
                    Employee = emp,
                    LastExamDate = exams.Where(ex => ex.EmployeeId == emp.Id)
                                        .OrderByDescending(ex => ex.ExamDate)
                                        .Select(ex => ex.ExamDate.ToString("dd.MM.yyyy"))
                                        .FirstOrDefault() ?? "—"
                }).OrderBy(x => x.Employee.LastName).ToList();

                dgEmployees.ItemsSource = displayList;
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is EmployeeExamStatus status)
            {
                SelectedEmployee = status.Employee;
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
