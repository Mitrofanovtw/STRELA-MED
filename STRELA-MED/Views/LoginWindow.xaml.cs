using STRELA_MED.Data;
using STRELA_MED.Services;
using System.Windows;
using STRELA_MED.Models;

namespace STRELA_MED.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Employees.AsEnumerable().FirstOrDefault(u =>
                u.Login != null &&
                u.Password != null &&
                u.Login.Trim().ToLower() == login.ToLower() &&
                u.Password.Trim() == password);

                if (user != null)
                {
                    UserSession.CurrentUserId = user.Id;
                    UserSession.CurrentUserName = $"{user.LastName} {user.FirstName[0]}. {user.MiddleName[0]}.";
                    UserSession.CurrentRole = user.Role;
                    CurrentUser.Data = user;
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль. Обратитесь к администратору медпункта.",
                                    "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}