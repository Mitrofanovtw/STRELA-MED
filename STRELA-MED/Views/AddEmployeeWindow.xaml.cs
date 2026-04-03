using STRELA_MED.Models;
using System.Windows;

namespace STRELA_MED.Views
{
    public partial class AddEmployeeWindow : Window
    {
        public Employee CurrentEmployee { get; private set; }
        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        public AddEmployeeWindow(Employee employee) : this()
        {
            CurrentEmployee = employee;
            Title = "Личная карточка / Редактирование";

            LastNameInput.Text = employee.LastName;
            FirstNameInput.Text = employee.FirstName;
            MiddleNameInput.Text = employee.MiddleName;
            BirthDatePicker.SelectedDate = employee.BirthDate;
            PositionBox.Text = employee.Position;
            ChronicDiseasesInput.Text = employee.ChronicDiseases;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}