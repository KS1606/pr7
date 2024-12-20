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
    /// Логика взаимодействия для GlavWindow.xaml
    /// </summary>
    public partial class GlavWindow : Window
    {
        private int userId;

        public GlavWindow(int UserId)
        {
            InitializeComponent();
            this.userId = UserId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoctorsListWindow admin = new DoctorsListWindow(userId);
            Close();
            admin.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PatientProfileWindow admin = new PatientProfileWindow(userId);
            Close();
            admin.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            Close();
            main.Show();
        }
    }
}
