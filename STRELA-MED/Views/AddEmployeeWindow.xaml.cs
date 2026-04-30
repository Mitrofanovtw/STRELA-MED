using STRELA_MED.Models;
using System.Windows;
using STRELA_MED.Data;
using System;

namespace STRELA_MED.Views
{
    public partial class AddEmployeeWindow : Window
    {
        public Employee CurrentEmployee { get; private set; }
        public AddEmployeeWindow()
        {
            InitializeComponent();
            FillPositions();
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
            if (string.IsNullOrWhiteSpace(LastNameInput.Text) || string.IsNullOrWhiteSpace(FirstNameInput.Text))
            {
                MessageBox.Show("Заполните фамилию и имя сотрудника!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    string translitLastName = Transliterate(LastNameInput.Text.ToLower());
                    string translitFirstName = Transliterate(FirstNameInput.Text.ToLower().Substring(0, 1));
                    string generatedLogin = $"{translitLastName}_{translitFirstName}";

                    var newEmployee = new Employee
                    {
                        LastName = LastNameInput.Text,
                        FirstName = FirstNameInput.Text,
                        MiddleName = MiddleNameInput.Text,
                        BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now.AddYears(-20),
                        Position = (PositionBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString(),
                        ChronicDiseases = ChronicDiseasesInput.Text,
                        Login = generatedLogin,
                        Password = "password123",
                        Role = "User"
                    };

                    db.Employees.Add(newEmployee);
                    db.SaveChanges();

                    if (NeedMedicalExamCheck.IsChecked == true)
                    {
                        var notification = new Notification
                        {
                            EmployeeId = newEmployee.Id,
                            Message = "СИСТЕМА: Направление на первичный медицинский осмотр при приёме на работу. Пожалуйста, явитесь в медпункт.",
                            AppointmentDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                            IsRead = false
                        };
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                }

                MessageBox.Show("Сотрудник успешно добавлен и направлен на осмотр!", "Успех");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private string Transliterate(string text)
        {
            var cyrillicToLatin = new Dictionary<char, string>
            {
                {'а',"a"}, {'б',"b"}, {'в',"v"}, {'г',"g"}, {'д',"d"}, {'е',"e"}, {'ё',"yo"},
                {'ж',"zh"}, {'з',"z"}, {'и',"i"}, {'й',"y"}, {'к',"k"}, {'л',"l"}, {'м',"m"},
                {'н',"n"}, {'о',"o"}, {'п',"p"}, {'р',"r"}, {'с',"s"}, {'т',"t"}, {'у',"u"},
                {'ф',"f"}, {'х',"kh"}, {'ц',"ts"}, {'ч',"ch"}, {'ш',"sh"}, {'щ',"shch"},
                {'ъ',""}, {'ы',"y"}, {'ь',""}, {'э',"e"}, {'ю',"yu"}, {'я',"ya"}
            };

            string result = "";
            foreach (char c in text)
            {
                result += cyrillicToLatin.ContainsKey(c) ? cyrillicToLatin[c] : c.ToString();
            }
            return result;
        }

        private void FillPositions()
        {
            var positions = new List<string>
            {
                // Руководство и мастера
                "Начальник цеха", "Мастер участка", "Старший мастер", "Диспетчер производства",
        
                // Инженерно-технические работники
                "Инженер-конструктор", "Инженер-технолог", "Инженер по качеству",
                "Инженер-программист станков с ЧПУ", "Инженер по нормированию труда",
                "Специалист по охране труда", "Специалист отдела кадров", "Лаборант химического анализа",
        
                // Основные рабочие специальности
                "Слесарь-сборщик летательных аппаратов", "Оператор станков с ПУ",
                "Токарь-карусельщик", "Токарь-расточник", "Фрезеровщик", "Шлифовщик",
                "Слесарь по КИПиА", "Слесарь-инструментальщик", "Слесарь-ремонтник",
                "Клепальщик", "Разметчик", "Медник", "Жестянщик",
        
                // Сварка и термообработка
                "Электросварщик на автоматических машинах", "Газосварщик",
                "Термист", "Гальваник", "Литейщик металлов и сплавов", "Кузнец-штамповщик",
        
                // Контроль и испытания
                "Контролер станочных и слесарных работ (ОТК)", "Дефектоскопист",
                "Испытатель агрегатов и приборов", "Геодезист",
        
                // Обслуживание и логистика
                "Электромонтер", "Маляр авиационный", "Стропальщик",
                "Водитель погрузчика", "Кладовщик", "Подсобный рабочий",
        
                // Администрация и охрана
                "Бухгалтер", "Юрисконсульт", "Техник по защите информации",
                "Системный администратор", "Переводчик", "Пожарный", "Охранник",
                "Уборщик производственных помещений"
            };

            PositionBox.ItemsSource = positions.OrderBy(p => p).ToList();
        }
    }
}