using System;
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class MainWindow : Window
    {
        LoginDataTableAdapter login_data = new LoginDataTableAdapter();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var login_data_tab = login_data.GetData().Rows;
            bool isAuthenticated = false;  // Флаг для отслеживания успешной авторизации
            int userId = 0;  // Переменная для хранения userId

            for (int i = 0; i < login_data_tab.Count; i++)
            {
                if (login_data_tab[i][1].ToString() == login.Text && login_data_tab[i][2].ToString() == password.Password)
                {
                    // Получаем userId (можно использовать этот ID для дальнейшей работы)
                    userId = Convert.ToInt32(login_data_tab[i][0]);  // Допустим, ID пользователя хранится в первой колонке
                    string roleId = (string)login_data_tab[i][3];

                    switch (roleId)
                    {
                        case "Врач":
                            isAuthenticated = true;  // Авторизация прошла успешно
                            GlavDoctorWindow admin = new GlavDoctorWindow(userId);  // Передаем userId в окно
                            Close();
                            admin.Show();
                            break;
                        case "Пользователь":
                            isAuthenticated = true;  // Авторизация прошла успешно
                            GlavWindow admin1 = new GlavWindow(userId);  // Передаем userId в окно
                            Close();
                            admin1.Show();
                            break;
                        case "Администратор":
                            isAuthenticated = true;  // Авторизация прошла успешно
                            GlavAndinWindow admin2 = new GlavAndinWindow(userId);  // Передаем userId в окно
                            Close();
                            admin2.Show();
                            break;

                    }

                    break;
                }
            }

            // Если авторизация не прошла
            if (!isAuthenticated)
            {
                MessageBox.Show("Неправильно введен логин или пароль.\nПопробуйте еще раз!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            Close();
            registerWindow.Show();
        }
    }
}
