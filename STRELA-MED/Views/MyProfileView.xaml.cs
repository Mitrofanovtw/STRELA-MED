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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STRELA_MED.Views
{
    /// <summary>
    /// Логика взаимодействия для MyProfileView.xaml
    /// </summary>
    public partial class MyProfileView : UserControl
    {
        public MyProfileView()
        {
            InitializeComponent();
            this.DataContext = CurrentUser.Data;
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string filePath = dlg.FileName;

                    using (var db = new AppDbContext())
                    {
                        var user = db.Employees.Find(CurrentUser.Data.Id);
                        if (user != null)
                        {
                            user.PhotoPath = filePath;
                            db.SaveChanges();

                            
                            ProfileImage.Source = new BitmapImage(new Uri(filePath));

                            MessageBox.Show("Фотография успешно обновлена!", "Успех");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке фото: " + ex.Message);
                }
            }
        }
    }
}
