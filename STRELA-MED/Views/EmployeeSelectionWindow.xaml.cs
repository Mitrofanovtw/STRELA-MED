using STRELA_MED.Data;
using STRELA_MED.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace STRELA_MED.Views
{
    public partial class EmployeeSelectionWindow : Window
    {
        public Employee SelectedEmployee { get; private set; }
        private List<Employee> _allEmployees;

        public EmployeeSelectionWindow()
        {
            InitializeComponent();
            LoadEmployees();
            tbSearch.Focus();
        }

        private void LoadEmployees()
        {
            using (var db = new AppDbContext())
            {
                _allEmployees = db.Employees.OrderBy(e => e.LastName).ToList();
                dgEmployees.ItemsSource = _allEmployees;
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgEmployees.ItemsSource = _allEmployees;
            }
            else
            {
                dgEmployees.ItemsSource = _allEmployees
                    .Where(emp => emp.LastName.ToLower().Contains(searchText) ||
                                  emp.FirstName.ToLower().Contains(searchText))
                    .ToList();
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee emp)
            {
                SelectedEmployee = emp;
                DialogResult = true;
            }
            else MessageBox.Show("Выберите сотрудника из списка!");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void dgEmployees_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => Select_Click(null, null);
    }
}