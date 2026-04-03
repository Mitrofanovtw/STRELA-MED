using STRELA_MED.Data;
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
using System.Windows.Shapes;

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для EmployeeCardWindow.xaml
    /// </summary>
    public partial class EmployeeCardWindow : Window
    {
        public EmployeeCardWindow()
        {
            InitializeComponent();
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            var emp = this.DataContext as Employee;

            if (emp != null)
            {
                using (var db = new AppDbContext())
                {
                    var dbEmp = db.Employees.FirstOrDefault(x => x.Id == emp.Id);
                    if (dbEmp != null)
                    {
                        dbEmp.LastName = emp.LastName;
                        dbEmp.FirstName = emp.FirstName;
                        dbEmp.MiddleName = emp.MiddleName;
                        dbEmp.Position = emp.Position;
                        dbEmp.ChronicDiseases = emp.ChronicDiseases;

                        db.SaveChanges();
                        MessageBox.Show("Данные успешно обновлены!");
                        this.Close();
                    }
                }
            }
        }
    }
}
