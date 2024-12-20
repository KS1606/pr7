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
using System.Windows.Shapes;

namespace MedicalInfo
{
    /// <summary>
    /// Логика взаимодействия для GlavDoctorWindow.xaml
    /// </summary>
    public partial class GlavDoctorWindow : Window
    {
        private int userId;
        public GlavDoctorWindow(int id)
        {
            InitializeComponent();
            this.userId = id;
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoctorProfileWindow window = new DoctorProfileWindow(userId);
            Close();
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PatientListWindow window = new PatientListWindow(userId);
            Close();
            window.Show();
        }
    }
}
