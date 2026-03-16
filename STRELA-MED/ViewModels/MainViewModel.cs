using STRELA_MED.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace STRELA_MED.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        private User _currentUser;

        public object CurrentView { get => _currentView; set { _currentView = value; OnPropertyChanged(); } }

        public User CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); } }

        public ICommand ShowEmployeesCommand { get; }
        public ICommand ShowInventoryCommand { get; }

        public MainViewModel(User user)
        {
            _currentUser = user;

            ShowEmployeesCommand = new RelayCommand(o => CurrentView = new EmployeeListViewModel(_currentUser));
            ShowInventoryCommand = new RelayCommand(o => CurrentView = new InventoryViewModel(_currentUser));

            CurrentView = new EmployeeListViewModel(_currentUser);
        }//влоарыва для коммита

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}