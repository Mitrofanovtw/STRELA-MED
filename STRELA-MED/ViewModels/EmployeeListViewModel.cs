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
        private Employee _currentUser;
        private ObservableCollection<Employee> _employees;
        private Employee _selectedEmployee;
        private string _searchText;
        private ObservableCollection<Employee> _allEmployees;

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

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ICommand AddEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand RefreshCommand { get; }

        public EmployeeListViewModel(Employee user)
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
                var list = db.Employees.ToList();
                _allEmployees = new ObservableCollection<Employee>(list);
                Employees = new ObservableCollection<Employee>(list);
            }
        }

        private void ApplyFilter()
        {
            if (_allEmployees == null) return;
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Employees = new ObservableCollection<Employee>(_allEmployees);
                return;
            }

            var search = SearchText.ToLower().Trim();

            var filtered = _allEmployees.Where(e => {
                if (SelectedSearchIndex == 0)
                    return e.LastName != null && e.LastName.ToLower().StartsWith(search);

                else
                    return e.Position != null && e.Position.ToLower().Contains(search);
            }).ToList();

            Employees = new ObservableCollection<Employee>(filtered);
        }

        private bool _searchByLastName = true;
        public bool SearchByLastName
        {
            get => _searchByLastName;
            set { _searchByLastName = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private int _selectedSearchIndex = 0; // 0 - Фамилия, 1 - Должность
        public int SelectedSearchIndex
        {
            get => _selectedSearchIndex;
            set
            {
                _selectedSearchIndex = value;
                OnPropertyChanged();
                ApplyFilter();
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
                        DateTime birthDate = addWindow.BirthDatePicker.SelectedDate ?? DateTime.Now;

                        var newEmp = new Employee
                        {
                            LastName = addWindow.LastNameInput.Text?.Trim(),
                            FirstName = addWindow.FirstNameInput.Text?.Trim(),
                            MiddleName = addWindow.MiddleNameInput.Text?.Trim(),
                            BirthDate = DateTime.SpecifyKind(addWindow.BirthDatePicker.SelectedDate ?? DateTime.Now, DateTimeKind.Utc),
                            Position = addWindow.PositionBox.Text,

                            ChronicDiseases = string.IsNullOrWhiteSpace(addWindow.ChronicDiseasesInput.Text) ? "Нет" : addWindow.ChronicDiseasesInput.Text.Trim()
                        };

                        db.Employees.Add(newEmp);
                        db.SaveChanges();
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    Exception realEx = ex;
                    while (realEx.InnerException != null) realEx = realEx.InnerException;
                    MessageBox.Show($"ОШИБКА БАЗЫ: {realEx.Message}");
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
                    var empToDelete = db.Employees.Find(SelectedEmployee.Id);
                    if (empToDelete != null)
                    {
                        db.Employees.Remove(empToDelete);
                        db.SaveChanges();
                    }
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

        private void OpenEditEmployeeWindow()
        {
            if (SelectedEmployee == null) return;

            var editWindow = new AddEmployeeWindow(SelectedEmployee);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var emp = db.Employees.Find(SelectedEmployee.Id);
                        if (emp != null)
                        {
                            emp.LastName = editWindow.LastNameInput.Text;
                            emp.FirstName = editWindow.FirstNameInput.Text;
                            emp.MiddleName = editWindow.MiddleNameInput.Text;
                            emp.BirthDate = DateTime.SpecifyKind(editWindow.BirthDatePicker.SelectedDate ?? DateTime.Now, DateTimeKind.Utc);
                            emp.Position = editWindow.PositionBox.Text;
                            emp.ChronicDiseases = string.IsNullOrWhiteSpace(editWindow.ChronicDiseasesInput.Text) ? "Нет" : editWindow.ChronicDiseasesInput.Text.Trim();

                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        public ICommand OpenCardCommand => new RelayCommand(o => {
            if (SelectedEmployee != null)
            {
                var card = new EmployeeCardWindow();
                card.DataContext = SelectedEmployee;
                card.ShowDialog();
            }
        });
    }
}