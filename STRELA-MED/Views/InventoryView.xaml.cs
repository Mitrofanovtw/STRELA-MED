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
using STRELA_MED.Data;
using STRELA_MED.Models;

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        private List<Medicine> _allMedicines = new List<Medicine>();
        public InventoryView()
        {
            InitializeComponent();
            LoadData();
            ApplyPermissions();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                _allMedicines = db.Medicines.ToList();
                UpdateGrid();
            }
        }
        private void UpdateGrid()
        {
            string search = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                MedicineGrid.ItemsSource = _allMedicines;
            }
            else
            {
                MedicineGrid.ItemsSource = _allMedicines
                    .Where(m => m.Name != null && m.Name.ToLower().Contains(search))
                    .ToList();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void ApplyPermissions()
        {
            var user = CurrentUser.Data;
            if (user == null) return;

            if (user.Role == "Admin")
            {
                ActionQuantity.Visibility = Visibility.Collapsed;
            }
        }

        private void MedicineGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MedicineGrid.SelectedItem is Medicine selected)
            {
                SelectedMedicineText.Text = selected.Name;
            }
        }

        private void BtnArrival_Click(object sender, RoutedEventArgs e)
        {
            UpdateQuantity(true);
        }

        private void BtnUsage_Click(object sender, RoutedEventArgs e)
        {
            UpdateQuantity(false);
        }

        private void UpdateQuantity(bool isArrival)
        {
            if (MedicineGrid.SelectedItem is Medicine selected)
            {
                if (!int.TryParse(ActionQuantity.Text, out int changeValue) || changeValue <= 0)
                {
                    MessageBox.Show("Введите корректное положительное число!");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var medicineInDb = db.Medicines.Find(selected.Id);
                    if (medicineInDb != null)
                    {
                        if (isArrival)
                        {
                            medicineInDb.Quantity += changeValue;
                        }
                        else
                        {
                            if (medicineInDb.Quantity < changeValue)
                            {
                                MessageBox.Show("Недостаточно товара на складе!");
                                return;
                            }
                            medicineInDb.Quantity -= changeValue;
                        }

                        db.SaveChanges();
                        ActionQuantity.Clear();
                        LoadData();
                        MessageBox.Show("Данные успешно обновлены!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите препарат в таблице!");
            }
        }
    }
}

