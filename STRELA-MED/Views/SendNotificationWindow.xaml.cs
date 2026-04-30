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
    /// Логика взаимодействия для SendNotificationWindow.xaml
    /// </summary>
    public partial class SendNotificationWindow : Window
    {
        public string FinalMessage { get; private set; }

        public SendNotificationWindow(string defaultMessage)
        {
            InitializeComponent();
            tbMessage.Text = defaultMessage;
            tbMessage.Focus();
            tbMessage.SelectAll();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbMessage.Text))
            {
                MessageBox.Show("Сообщение не может быть пустым!");
                return;
            }
            FinalMessage = tbMessage.Text;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
