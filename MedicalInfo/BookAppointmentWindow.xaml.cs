using System;
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class BookAppointmentWindow : Window
    {
        private int patiId;
        private DateTime startDate = DateTime.Today; // Устанавливаем стартовую дату
        private MedicalHistoryTableAdapter medicalHistoryAdapter = new MedicalHistoryTableAdapter();
        private TreatmentRecordTableAdapter treatmentRecordsAdapter = new TreatmentRecordTableAdapter();
        private LoginDataTableAdapter loginDateAdapter = new LoginDataTableAdapter(); // Используем LoginDate для получения ФИО врача

        public BookAppointmentWindow(int patientId)
        {
            InitializeComponent();
            patiId = patientId;
            LoadDoctors();
        }

        // Загрузка врачей в ComboBox
        private void LoadDoctors()
        {
            try
            {
                // Фильтруем пользователей по роли "doctor"
                var doctors = loginDateAdapter.GetData()
                    .Where(user => user.role == "Врач") // Предполагаем, что поле "role" хранит информацию о роли пользователя
                    .ToList();

                DoctorComboBox.ItemsSource = doctors;
                DoctorComboBox.DisplayMemberPath = "full_name";  // Отображаем ФИО врача
                DoctorComboBox.SelectedValuePath = "id";  // Идентификатор врача
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке списка врачей: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки "Записаться на прием"
        private void ConfirmAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбраны ли врач и дата
            if (DoctorComboBox.SelectedItem == null || AppointmentDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранного врача и дату
            int doctorId = (int)DoctorComboBox.SelectedValue;
            DateTime appointmentDate = AppointmentDatePicker.SelectedDate.Value;

            // Проверяем, что дата не меньше стартовой
            if (appointmentDate < startDate)
            {
                MessageBox.Show($"Нельзя записаться на эту дату. Выберите дату начиная с {startDate:dd.MM.yyyy}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Ищем существующую запись в истории болезни пациента
                var medicalHistory = medicalHistoryAdapter.GetData().FirstOrDefault(mh => mh.patient_id == patiId);

                if (medicalHistory == null)
                {
                    // Если истории болезни нет, создаем новую запись
                    medicalHistoryAdapter.Insert(appointmentDate, appointmentDate, " ", patiId);
                    // Получаем только что добавленную запись
                    medicalHistory = medicalHistoryAdapter.GetData().FirstOrDefault(mh => mh.patient_id == patiId && mh.start_date == appointmentDate);
                }

                // Добавляем запись о приеме
                treatmentRecordsAdapter.Insert(appointmentDate, " ", medicalHistory.id, 9, doctorId);

                MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();  // Закрываем окно после успешной записи
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
