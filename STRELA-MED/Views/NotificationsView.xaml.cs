using Microsoft.EntityFrameworkCore;
using STRELA_MED.Data;
using STRELA_MED.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                var notifications = db.Notifications
                    .OrderByDescending(n => n.AppointmentDate)
                    .ToList();

                NotificationsGrid.ItemsSource = notifications;

                UpdateHeaderCount(notifications.Count);
            }
        }

        private void UpdateHeaderCount(int count)
        {
            txtHeader.Text = $"ВАШИ УВЕДОМЛЕНИЯ ({count})";
        }

        private void NotificationsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (NotificationsGrid.SelectedItem is Notification selected)
            {
                var viewWindow = new ViewNotificationWindow();
                viewWindow.txtFullMessage.Text = selected.Message;
                viewWindow.Owner = Window.GetWindow(this);
                viewWindow.ShowDialog();
            }
        }

        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить ВСЕ уведомления? Это действие нельзя отменить.",
                "Подтверждение очистки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var userId = CurrentUser.Data.Id;
                        var allNotifications = db.Notifications.Where(n => n.EmployeeId == userId).ToList();

                        if (allNotifications.Any())
                        {
                            db.Notifications.RemoveRange(allNotifications);
                            db.SaveChanges();

                            MessageBox.Show("Список уведомлений очищен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadUserNotifications();
                        }
                        else
                        {
                            MessageBox.Show("Список и так пуст.", "Инфо");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при очистке: {ex.Message}", "Ошибка");
                }
            }
            using (var db = new AppDbContext())
            {
                var all = db.Notifications.ToList();
                db.Notifications.RemoveRange(all);
                db.SaveChanges();

                NotificationsGrid.ItemsSource = null;
                UpdateHeaderCount(0);
                MessageBox.Show("Список очищен!");
            }
        }
    }
}