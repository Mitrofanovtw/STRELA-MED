using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using STRELA_MED.Data;
using STRELA_MED.Models;
using STRELA_MED.Services;

namespace STRELA_MED.ViewModels
{
    public class MyExamsViewModel : INotifyPropertyChanged
    {
        private Employee _currentUser;
        private ObservableCollection<MedicalExam> _exams;
        private int _totalEmployees;
        private int _pendingExams;
        private int _lowStockItems;

        public int TotalEmployees { get => _totalEmployees; set { _totalEmployees = value; OnPropertyChanged(); } }
        public int PendingExams { get => _pendingExams; set { _pendingExams = value; OnPropertyChanged(); } }
        public int LowStockItems { get => _lowStockItems; set { _lowStockItems = value; OnPropertyChanged(); } }

        public ObservableCollection<MedicalExam> Exams
        {
            get => _exams;
            set
            {
                _exams = value;
                OnPropertyChanged();
            }
        }

        public MyExamsViewModel(Employee user)
        {
            _currentUser = user;
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userExams = db.MedicalExams
                        .Where(e => e.EmployeeId == _currentUser.Id)
                        .OrderByDescending(e => e.ExamDate)
                        .ToList();

                    Exams = new ObservableCollection<MedicalExam>(userExams);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории осмотров: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}