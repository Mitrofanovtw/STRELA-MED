using STRELA_MED.Data;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STRELA_MED.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        public int TotalEmployees { get; set; }
        public int CompletedExams { get; set; }
        public int PendingExams { get; set; }
        public int LowStockItems { get; set; }

        public ReportsViewModel()
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var db = new AppDbContext())
            {
                TotalEmployees = db.Employees.Count();
                CompletedExams = db.MedicalExams.Count(e => e.Result != null && e.Result != "");
                PendingExams = db.MedicalExams.Count(e => e.Result == null || e.Result == "");
                LowStockItems = db.Medicines.Count(m => m.Quantity < 10);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}