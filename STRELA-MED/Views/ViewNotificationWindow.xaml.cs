using System.Windows;

namespace STRELA_MED.Views
{
    public partial class ViewNotificationWindow : Window
    {
        public ViewNotificationWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}