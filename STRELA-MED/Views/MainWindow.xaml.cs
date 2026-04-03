using STRELA_MED.Data;
using STRELA_MED.Services;
using STRELA_MED.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using STRELA_MED.Models;

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (CurrentUser.Data != null)
            {
                this.DataContext = new MainViewModel(CurrentUser.Data);
                string role = CurrentUser.Data.Role.ToLower();
                if (role == "admin" || role == "doctor")
                {
                    AdminPanel.Visibility = Visibility.Visible;
                    UserPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AdminPanel.Visibility = Visibility.Collapsed;
                    UserPanel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
        private void Logout()
        {
            CurrentUser.Data = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?",
                                         "Смена пользователя",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CurrentUser.Data = null;

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть приложение?",
                                         "Выход",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
