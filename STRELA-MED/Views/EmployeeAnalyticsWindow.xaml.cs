using LiveCharts;
using LiveCharts.Wpf;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace STRELA_MED.Views
{
    public partial class EmployeeAnalyticsWindow : Window
    {
        public ChartValues<double> PulseValues { get; set; }
        public ChartValues<double> SysPressureValues { get; set; }
        public ChartValues<double> DiaPressureValues { get; set; }
        public List<string> DateLabels { get; set; }

        public EmployeeAnalyticsWindow(Employee employee)
        {
            PulseValues = new ChartValues<double>();
            SysPressureValues = new ChartValues<double>();
            DiaPressureValues = new ChartValues<double>();
            DateLabels = new List<string>();

            InitializeComponent();

            txtFullName.Text = $"{employee.LastName} {employee.FirstName} {employee.MiddleName}";
            txtPosition.Text = $"Должность: {employee.Position}";

            LoadChartData(employee.Id);
            this.DataContext = this;
        }

        private void LoadChartData(int employeeId)
        {
            using (var db = new AppDbContext())
            {
                var exams = db.MedicalExams
                    .Where(e => e.EmployeeId == employeeId)
                    .OrderBy(e => e.ExamDate)
                    .ToList();

                PulseValues.Clear();
                SysPressureValues.Clear();
                DiaPressureValues.Clear();
                DateLabels.Clear();

                foreach (var exam in exams)
                {
                    double pulse = Convert.ToDouble(exam.Pulse ?? 0);
                    PulseValues.Add(pulse);

                    DateLabels.Add(exam.ExamDate.ToString("dd.MM"));

                    if (!string.IsNullOrEmpty(exam.BloodPressure) && exam.BloodPressure.Contains("/"))
                    {
                        var parts = exam.BloodPressure.Split('/');
                        if (double.TryParse(parts[0], out double sys)) SysPressureValues.Add(sys);
                        if (double.TryParse(parts[1], out double dia)) DiaPressureValues.Add(dia);
                    }
                    else
                    {
                        SysPressureValues.Add(0);
                        DiaPressureValues.Add(0);
                    }
                }
            }
        }
    }
}