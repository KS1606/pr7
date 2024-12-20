using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Для работы с SQL-запросами
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class EditTreatmentRecordWindow : Window
    {
        private readonly TreatmentRecordModel _treatmentRecord; // Передается через конструктор
        private readonly DiagnosesTableAdapter _diagnosesAdapter = new DiagnosesTableAdapter();
        private readonly MedicalServicesTableAdapter _servicesAdapter = new MedicalServicesTableAdapter();
        private readonly MedicationsTableAdapter _medicationsAdapter = new MedicationsTableAdapter();

        // Строка подключения к базе данных (замените на вашу реальную строку подключения)
        private const string ConnectionString = "Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True";

        public EditTreatmentRecordWindow(TreatmentRecordModel treatmentRecord)
        {
            InitializeComponent();
            _treatmentRecord = treatmentRecord;

            // Привязываем данные модели для редактирования
            this.DataContext = _treatmentRecord;

            // Загружаем данные для комбобоксов
            LoadDropdownData();
        }

        
        // Загружаем данные для комбобоксов
        private void LoadDropdownData()
        {
            try
            {
                // Загружаем диагнозы
                var diagnoses = _diagnosesAdapter.GetData();
                DiagnosisComboBox.ItemsSource = diagnoses;
                DiagnosisComboBox.DisplayMemberPath = "name"; // Убедитесь, что это имя колонки в базе данных
                DiagnosisComboBox.SelectedValuePath = "id";   // Убедитесь, что это имя колонки в базе данных
                DiagnosisComboBox.SelectedValue = _treatmentRecord.DiagnosisId;

                // Загружаем доступные медицинские услуги
                var services = _servicesAdapter.GetData();
                ServicesComboBox.ItemsSource = services;
                ServicesComboBox.DisplayMemberPath = "name"; // Имя услуги
                ServicesComboBox.SelectedValuePath = "id";   // Идентификатор услуги

                // Загружаем доступные препараты
                var medications = _medicationsAdapter.GetData();
                MedicationsComboBox.ItemsSource = medications;
                MedicationsComboBox.DisplayMemberPath = "name"; // Название препарата
                MedicationsComboBox.SelectedValuePath = "id";   // Идентификатор препарата
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ServicesComboBox.SelectedValue != null)
                {
                    int selectedServiceId = Convert.ToInt32(ServicesComboBox.SelectedValue);

                    // Добавляем услугу в запись
                    AddServiceToRecord(selectedServiceId);

                    MessageBox.Show("Услуга успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите услугу для добавления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении услуги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddServiceToRecord(int serviceId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // SQL-запрос для добавления услуги в назначенные услуги
                const string insertQuery = @"
                    INSERT INTO [dbo].[AssignedServices] ([record_id], [service_id])
                    VALUES (@RecordId, @ServiceId)";

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    // Передаем параметры в запрос
                    command.Parameters.AddWithValue("@RecordId", _treatmentRecord.Id);
                    command.Parameters.AddWithValue("@ServiceId", serviceId);

                    // Выполняем запрос
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем модель на основе пользовательских данных
                _treatmentRecord.Description = DescriptionTextBox.Text;
                _treatmentRecord.Date = DateDatePicker.SelectedDate ?? DateTime.Now; // Подстраховка от null

                if (DiagnosisComboBox.SelectedValue != null)
                    _treatmentRecord.DiagnosisId = Convert.ToInt32(DiagnosisComboBox.SelectedValue);

                // Проверяем, что описание не пустое
                if (string.IsNullOrWhiteSpace(_treatmentRecord.Description))
                {
                    MessageBox.Show("Описание не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Выполняем обновление записи через SQL-запрос
                UpdateTreatmentRecordInDatabase(_treatmentRecord);

                MessageBox.Show("Запись успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMedicationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MedicationsComboBox.SelectedValue != null)
                {
                    int selectedMedicationId = Convert.ToInt32(MedicationsComboBox.SelectedValue);

                    // Добавляем препарат в запись
                    AddMedicationToRecord(selectedMedicationId);

                    MessageBox.Show("Препарат успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите препарат для добавления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении препарата: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMedicationToRecord(int medicationId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // SQL-запрос для добавления препарата в назначения
                const string insertQuery = @"
            INSERT INTO [dbo].[Prescriptions] ([record_id], [medication_id])
            VALUES (@RecordId, @MedicationId)";

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    // Передаем параметры в запрос
                    command.Parameters.AddWithValue("@RecordId", _treatmentRecord.Id);
                    command.Parameters.AddWithValue("@MedicationId", medicationId);

                    // Выполняем запрос
                    command.ExecuteNonQuery();
                }
            }
        }


        private void UpdateTreatmentRecordInDatabase(TreatmentRecordModel treatmentRecord)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // SQL-запрос для обновления записи
                const string updateQuery = @"
                    UPDATE [dbo].[TreatmentRecord]
                    SET 
                        [date] = @Date,
                        [description] = @Description,
                        [diagnosis_id] = @DiagnosisId
                    WHERE 
                        [id] = @Id";

                using (var command = new SqlCommand(updateQuery, connection))
                {
                    // Передаем параметры в запрос
                    command.Parameters.AddWithValue("@Date", treatmentRecord.Date);
                    command.Parameters.AddWithValue("@Description", treatmentRecord.Description);
                    command.Parameters.AddWithValue("@DiagnosisId", treatmentRecord.DiagnosisId);
                    command.Parameters.AddWithValue("@Id", treatmentRecord.Id);

                    // Выполняем запрос
                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Запись не была обновлена. Возможно, запись с указанным ID не существует.");
                    }
                }
            }
        }
    }
}
