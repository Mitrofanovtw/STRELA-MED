using Microsoft.EntityFrameworkCore;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System.Linq;
using System.Windows.Controls;

namespace STRELA_MED.Views
{
    public partial class NotificationsView : UserControl
    {
        public NotificationsView()
        {
            InitializeComponent();
            LoadUserNotifications();
        }

        private void LoadUserNotifications()
        {
            using (var db = new AppDbContext())
            {
                var myId = CurrentUser.Data.Id;
                var list = db.Notifications
                             .Where(n => n.EmployeeId == myId)
                             .OrderByDescending(n => n.AppointmentDate)
                             .ToList();

                NotificationsGrid.ItemsSource = list;
            }
        }
    }
}