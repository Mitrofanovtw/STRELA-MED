using System.Windows;
using STRELA_MED.Services;

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
            var user = AuthService.Authenticate(LoginBox.Text, PasswordBox.Password);

            if (user != null)
            {
                MainWindow main = new MainWindow(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка входа",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}