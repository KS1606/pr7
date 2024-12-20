using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class DoctorsListWindow : Window
    {
        private int userId;
        private IEnumerable<dynamic> allDoctors; // Полный список врачей

        public DoctorsListWindow(int UserId)
        {
            InitializeComponent();
            this.userId = UserId;
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            // Создаем адаптеры для каждой таблицы
            var doctorsAdapter = new DoctorsTableAdapter();
            var specializationAdapter = new SpecializationTableAdapter();
            var logindataAdapter = new LoginDataTableAdapter();

            // Заполняем таблицы данными
            var doctorsTable = new MedicalSystemDataSet.DoctorsDataTable();
            var specializationTable = new MedicalSystemDataSet.SpecializationDataTable();
            var logindataTable = new MedicalSystemDataSet.LoginDataDataTable();

            doctorsAdapter.Fill(doctorsTable);
            specializationAdapter.Fill(specializationTable);
            logindataAdapter.Fill(logindataTable);

            // Присоединяем специализации и логин-данные к врачам с помощью LINQ
            allDoctors = from doctor in doctorsTable
                         join specialization in specializationTable
                         on doctor.specialization_id equals specialization.id
                         join loginData in logindataTable
                         on doctor.login_data_id equals loginData.id
                         select new
                         {
                             FullName = loginData.full_name,
                             SpecializationName = specialization.name,
                             Experience = doctor.experience,
                             ContactInfo = doctor.contact_info,
                             Schedule = doctor.schedule
                         };

            // Привязываем список к ItemsControl
            DoctorsListView.ItemsSource = allDoctors.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlavWindow admin = new GlavWindow(userId);
            Close();
            admin.Show();
        }

        private void FilterDoctors(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                // Если фильтр пустой, показываем всех врачей
                DoctorsListView.ItemsSource = allDoctors.ToList();
            }
            else
            {
                // Применяем фильтр
                var filteredDoctors = allDoctors.Where(doctor =>
                    doctor.FullName.ToLower().Contains(filterText.ToLower()) ||
                    doctor.SpecializationName.ToLower().Contains(filterText.ToLower())).ToList();

                DoctorsListView.ItemsSource = filteredDoctors;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст из текстового поля фильтра
            string filterText = FilterTextBox.Text;
            FilterDoctors(filterText);
        }
    }
}
