using STRELA_MED.Models;
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

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для EmployeeListView.xaml
    /// </summary>
    public partial class EmployeeListView : UserControl
    {
        public EmployeeListView()
        {
            InitializeComponent();
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            var user = CurrentUser.Data;
            if (user == null) return;

            if (user.Role != "Admin")
            {
                BtnAdd.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
            }
        }
    }
}
