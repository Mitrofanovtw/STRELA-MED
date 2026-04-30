using STRELA_MED.Services;
using STRELA_MED.Views;
using STRELA_MED.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace STRELA_MED.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        private Employee _currentUser;

        public object CurrentView { get => _currentView; set { _currentView = value; OnPropertyChanged(); } }

        public Employee CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); } }

        public ICommand ShowEmployeesCommand { get; }
        public ICommand ShowInventoryCommand { get; }
        public ICommand ShowMedicalExamsCommand { get; }
        public ICommand ShowMyProfileCommand { get; }
        public ICommand ShowNotificationsCommand { get; }
        public ICommand ShowReportsCommand { get; }
        public ICommand ShowMyExamsCommand => new RelayCommand(o =>
        {
            CurrentView = new MyExamsViewModel(CurrentUser);
        });

        public MainViewModel(Employee user)
        {
            _currentUser = user;
            ShowEmployeesCommand = new RelayCommand(o => CurrentView = new EmployeeListViewModel(_currentUser));
            ShowInventoryCommand = new RelayCommand(o => CurrentView = new InventoryViewModel(_currentUser));
            ShowMedicalExamsCommand = new RelayCommand(o => CurrentView = new MedicalExamsViewModel());
            
            ShowMyProfileCommand = new RelayCommand(o => {
                var view = new MyProfileView();
                view.DataContext = _currentUser;
                CurrentView = view;
            });

            ShowNotificationsCommand = new RelayCommand(o => {
                CurrentView = new NotificationsView();
            });
            if (_currentUser.Role == "Admin" || _currentUser.Role == "Doctor")
            {
                CurrentView = new EmployeeListViewModel(_currentUser);
            }
            else
            {
                var view = new MyProfileView();
                view.DataContext = _currentUser;
                CurrentView = view;
            }

            ShowReportsCommand = new RelayCommand(o => CurrentView = new ReportsViewModel());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}