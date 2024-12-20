using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class PatientListWindow : Window
    {
        private int userId;
        private IEnumerable<dynamic> allPatients; // Полный список пациентов

        public PatientListWindow(int UserId)
        {
            InitializeComponent();
            this.userId = UserId;
            LoadPatients();
        }

        private void LoadPatients()
        {
            // Создаем адаптеры для каждой таблицы
            var patientsAdapter = new PatientsTableAdapter();
            var logindataAdapter = new LoginDataTableAdapter();

            // Заполняем таблицы данными
            var patientsTable = new MedicalSystemDataSet.PatientsDataTable();
            var logindataTable = new MedicalSystemDataSet.LoginDataDataTable();

            patientsAdapter.Fill(patientsTable);
            logindataAdapter.Fill(logindataTable);

            // Присоединяем логин-данные к пациентам с помощью LINQ
            allPatients = from patient in patientsTable
                          join loginData in logindataTable
                          on patient.login_data_id equals loginData.id
                          select new
                          {
                              LoginDataId = patient.login_data_id,
                              FullName = loginData.full_name,
                              Dob = patient.dob,
                              Status = patient.status,
                              Phone = patient.phone
                          };

            // Привязываем список к ItemsControl
            PatientsListView.ItemsSource = allPatients.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlavWindow admin = new GlavWindow(userId);
            Close();
            admin.Show();
        }

        private void FilterPatients(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                // Если фильтр пустой, показываем всех пациентов
                PatientsListView.ItemsSource = allPatients.ToList();
            }
            else
            {
                // Применяем фильтр
                var filteredPatients = allPatients.Where(patient =>
                    patient.FullName.ToLower().Contains(filterText.ToLower()) ||
                    patient.Status.ToLower().Contains(filterText.ToLower())).ToList();

                PatientsListView.ItemsSource = filteredPatients;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст из текстового поля фильтра
            string filterText = FilterTextBox.Text;
            FilterPatients(filterText);
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Получаем данные пациента из контекста элемента (карточки)
            var patient = (dynamic)((Border)sender).DataContext;

            // Открываем новое окно и передаем id пациента
            PatientDetailWindow detailsWindow = new PatientDetailWindow(patient.LoginDataId);
            detailsWindow.Show();
        }
    }
}
