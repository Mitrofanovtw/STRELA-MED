using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using STRELA_MED.Data;
using STRELA_MED.Models;
using STRELA_MED.Services;
using STRELA_MED.Views;

namespace STRELA_MED.ViewModels
{
    public class EmployeeListViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private ObservableCollection<Employee> _employees;
        private Employee _selectedEmployee;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        public ICommand AddEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand RefreshCommand { get; }

        public EmployeeListViewModel(User user)
        {
            _currentUser = user;

            AddEmployeeCommand = new RelayCommand(o => OpenAddEmployeeWindow());
            DeleteEmployeeCommand = new RelayCommand(o => DeleteEmployee());
            RefreshCommand = new RelayCommand(o => LoadData());

            LoadData();
        }

        public void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Employees.AsQueryable();

                if (_currentUser.Role == "doctor")
                    Employees = new ObservableCollection<Employee>(query.Where(e => e.UserId == _currentUser.UserId).ToList());
                else if (_currentUser.Role == "admin")
                    Employees = new ObservableCollection<Employee>(query.ToList());
                else
                    Employees = new ObservableCollection<Employee>();
            }
        }

        private void OpenAddEmployeeWindow()
        {
            var addWindow = new AddEmployeeWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var newEmp = new Employee
                        {
                            Name = addWindow.NameInput.Text,
                            Role = addWindow.RoleInput.Text,
                            UserId = int.Parse(addWindow.UserIdInput.Text)
                        };
                        db.Employees.Add(newEmp);
                        db.SaveChanges();
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
                }
            }
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    db.Employees.Remove(SelectedEmployee);
                    db.SaveChanges();
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}