using STRELA_MED.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace STRELA_MED.Views
{
    public partial class MyExamsView : UserControl
    {
        public MyExamsView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedExam = (sender as DataGrid).SelectedItem as MedicalExam;

            if (selectedExam != null)
            {
                ExamCardWindow card = new ExamCardWindow(selectedExam);
                card.ShowDialog();
            }
        }
    }
}