using STRELA_MED.Data;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STRELA_MED.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        private int _totalEmployees;
        private int _pendingExams;
        private int _lowStockItems;
        public int TotalEmployees { get => _totalEmployees; set { _totalEmployees = value; OnPropertyChanged(); } }
        public int PendingExams { get => _pendingExams; set { _pendingExams = value; OnPropertyChanged(); } }
        public int LowStockItems { get => _lowStockItems; set { _lowStockItems = value; OnPropertyChanged(); } }

        public ReportsViewModel()
        {
            LoadStatistics();
        }

        public void LoadStatistics()
        {
            using (var db = new AppDbContext())
            {
                TotalEmployees = db.Employees.Count();
                var today = DateTime.UtcNow;
                PendingExams = db.Employees.Count(e =>
                    !db.MedicalExams.Any(m => m.EmployeeId == e.Id) ||
                    db.MedicalExams.Where(m => m.EmployeeId == e.Id).Max(m => m.ValidUntil) < today);

                LowStockItems = db.Medicines.Count(m => m.Quantity < 10);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}