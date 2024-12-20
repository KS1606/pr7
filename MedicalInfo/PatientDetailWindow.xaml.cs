using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class PatientDetailWindow : Window
    {
        private int userId;
        private LoginDataTableAdapter loginDataAdapter = new LoginDataTableAdapter(); // Адаптер для таблицы LoginData
        private PatientsTableAdapter patientsDataAdapter = new PatientsTableAdapter(); // Адаптер для таблицы Patients
        private MedicalHistoryTableAdapter medicalHistoryAdapter = new MedicalHistoryTableAdapter();
        private DoctorsTableAdapter doctorsAdapter = new DoctorsTableAdapter();
        private TreatmentRecordTableAdapter treatmentRecordsAdapter = new TreatmentRecordTableAdapter(); // Адаптер для записей о лечении
        public ObservableCollection<TreatmentRecordModel> FutureTreatmentRecords { get; set; }
        public ObservableCollection<MedicalHistoryModel> MedicalHistoryCollection { get; set; }
        private int patiId;
        public PatientDetailWindow(int UserId)
        {
            InitializeComponent();
            this.userId = UserId;
            this.DataContext = this;  // Устанавливаем DataContext в саму себя
            LoadPatientData();
        }

        private void LoadPatientData()
        {
            try
            {
                // Получаем полное имя из таблицы LoginData
                var loginData = loginDataAdapter.GetData()
                    .FirstOrDefault(ld => ld.id == userId); // Поиск по id из LoginData

                if (loginData != null)
                {
                    // Устанавливаем полное имя из LoginData
                    FullNameTextBlock.Text = loginData.full_name;
                }
                else
                {
                    MessageBox.Show("Пациент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем данные из таблицы Patients по login_data_id
                var patientData = patientsDataAdapter.GetData()
                    .FirstOrDefault(p => p.login_data_id == userId); // Поиск по login_data_id

                // Если пациент найден, заполняем поля
                if (patientData != null)
                {
                    patiId = patientData.id;  // Устанавливаем правильный идентификатор пациента

                    DobTextBlock.Text = patientData.dob.ToString("dd.MM.yyyy");
                    GenderTextBlock.Text = patientData.gender;
                    AddressTextBlock.Text = patientData.address;
                    PhoneTextBlock.Text = patientData.phone;
                    StatusTextBlock.Text = patientData.status;

                    // Загружаем историю болезни
                    LoadMedicalHistory();
                }
                else
                {
                    MessageBox.Show("Нет данных о пациенте!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMedicalHistory()
        {
            try
            {
                MedicalHistoryCollection = new ObservableCollection<MedicalHistoryModel>();

                // Получаем данные истории болезни из адаптера по patient_id
                var medicalHistoryData = medicalHistoryAdapter.GetData()
                    .Where(mh => mh.patient_id == patiId)  // Фильтруем по правильному идентификатору пациента
                    .ToList();

                if (!medicalHistoryData.Any())
                {
                    MessageBox.Show("История болезни не найдена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Добавляем записи в коллекцию и устанавливаем значение Id
                foreach (var record in medicalHistoryData)
                {
                    MedicalHistoryCollection.Add(new MedicalHistoryModel
                    {
                        Id = record.id, // Устанавливаем значение Id для каждой записи
                        StartDate = record.start_date,
                        EndDate = record.end_date,
                        TestResults = record.test_results
                    });
                }

                // Устанавливаем DataContext для ItemsControl
                MedicalHistoryItemsControl.ItemsSource = MedicalHistoryCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки истории болезни: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadFutureTreatmentRecords();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedMedicalHistory = button.Tag as MedicalHistoryModel;

            // Проверка на наличие выбранной медицинской записи
            if (selectedMedicalHistory != null)
            {
                MedicalHistoryPage2 detailWindow = new MedicalHistoryPage2(selectedMedicalHistory.Id);
                detailWindow.Show();
            }
            else
            {
                MessageBox.Show("Не удалось загрузить информацию о медицинской истории.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFutureTreatmentRecords()
        {
            try
            {
                // Инициализируем коллекцию для будущих записей о лечении
                FutureTreatmentRecords = new ObservableCollection<TreatmentRecordModel>();

                // Получаем данные из истории болезни для текущего пациента по patient_id
                var medicalHistoryData = medicalHistoryAdapter.GetData()
                    .FirstOrDefault(mh => mh.patient_id == patiId);  // Ищем историю болезни для текущего пациента

                if (medicalHistoryData == null)
                {
                    MessageBox.Show("Нет записи о истории болезни для пациента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем записи о лечении, фильтруем на будущее по history_id
                var treatmentRecordsData = treatmentRecordsAdapter.GetData()
                    .Where(tr => tr.history_id == medicalHistoryData.id)  // Будущие записи, игнорируем время
                    .OrderBy(tr => tr.date)
                    .ToList();

                if (!treatmentRecordsData.Any())
                {
                    MessageBox.Show("Нет будущих записей о лечении.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Проходим по всем найденным записям о лечении
                foreach (var record in treatmentRecordsData)
                {
                    // Получаем информацию о враче из таблицы LoginData по doctor_id
                    var doctorData = loginDataAdapter.GetData()
                        .FirstOrDefault(ld => ld.id == record.doctor_id);  // Ищем врача по doctor_id

                    // Если врач найден, добавляем информацию в запись
                    if (doctorData != null)
                    {
                        FutureTreatmentRecords.Add(new TreatmentRecordModel
                        {
                            Date = record.date,  // Дата записи
                            DoctorFullName = doctorData.full_name  // ФИО врача из таблицы LoginData
                        });
                    }
                    else
                    {
                        // Если врача нет, то добавляем запись без ФИО врача
                        FutureTreatmentRecords.Add(new TreatmentRecordModel
                        {
                            Date = record.date,  // Дата
                            DoctorFullName = "Неизвестен"  // Если врача нет, выводим "Неизвестен"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки записей о лечении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteTreatmentRecordButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную запись о лечении, на которую нажали кнопку
            var button = (Button)sender;
            var selectedTreatmentRecord = button.Tag as TreatmentRecordModel;

            // Проверяем, была ли выбрана запись для удаления
            if (selectedTreatmentRecord != null)
            {
                // Запрашиваем подтверждение у пользователя
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись о лечении?",
                                              "Подтверждение удаления",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Warning);

                // Если пользователь подтвердил удаление
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем запись из базы данных с помощью адаптера
                        treatmentRecordsAdapter.DeleteQuery(selectedTreatmentRecord.Id);

                        // Убираем запись из коллекции (для обновления UI)
                        FutureTreatmentRecords.Remove(selectedTreatmentRecord);

                        // Отображаем сообщение об успешном удалении
                        MessageBox.Show("Запись о лечении успешно удалена.",
                                         "Удаление",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок при удалении
                        MessageBox.Show("Ошибка при удалении записи: " + ex.Message,
                                         "Ошибка",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Если не была выбрана запись для удаления
                MessageBox.Show("Не удалось найти запись для удаления.",
                                 "Ошибка",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }



        // Обработчик для кнопки "Записаться на прием"
        private void BookAppointmentButton_Click(object sender, RoutedEventArgs e)
        {

            BookAppointmentWindow bookAppointmentWindow = new BookAppointmentWindow(patiId);
            bookAppointmentWindow.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GlavWindow glavWindow = new GlavWindow(userId);
            Close();
            glavWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoctorsListWindow doctorProfileWindow = new DoctorsListWindow(userId);
            Close();
            doctorProfileWindow.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            Close();
            main.Show();
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Путь для сохранения файлов на рабочем столе
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                // Создание CSV-файла
                string csvFilePath = Path.Combine(desktopPath, "PatientData.csv");
                using (StreamWriter csvWriter = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    // Заголовки
                    csvWriter.WriteLine("FullName,Dob,Gender,Address,Phone,Status");
                    csvWriter.WriteLine($"{FullNameTextBlock.Text},{DobTextBlock.Text},{GenderTextBlock.Text}," +
                                        $"{AddressTextBlock.Text},{PhoneTextBlock.Text},{StatusTextBlock.Text}");

                    csvWriter.WriteLine();
                    csvWriter.WriteLine("Medical History:");
                    csvWriter.WriteLine("StartDate,EndDate,TestResults");
                    foreach (var record in MedicalHistoryCollection)
                    {
                        csvWriter.WriteLine($"{record.StartDate:dd.MM.yyyy},{record.EndDate:dd.MM.yyyy},{record.TestResults}");
                    }

                    csvWriter.WriteLine();
                    csvWriter.WriteLine("Future Treatment Records:");
                    csvWriter.WriteLine("Date,DoctorFullName");
                    foreach (var record in FutureTreatmentRecords)
                    {
                        csvWriter.WriteLine($"{record.Date:dd.MM.yyyy},{record.DoctorFullName}");
                    }
                }

                // Создание SQL-файла
                string sqlFilePath = Path.Combine(desktopPath, "PatientData.sql");
                using (StreamWriter sqlWriter = new StreamWriter(sqlFilePath, false, Encoding.UTF8))
                {
                    // SQL для данных пациента
                    sqlWriter.WriteLine("INSERT INTO Patients (FullName, Dob, Gender, Address, Phone, Status) VALUES");
                    sqlWriter.WriteLine($"('{FullNameTextBlock.Text}', '{DobTextBlock.Text}', '{GenderTextBlock.Text}', " +
                                        $"'{AddressTextBlock.Text}', '{PhoneTextBlock.Text}', '{StatusTextBlock.Text}');");

                    // SQL для истории болезни
                    sqlWriter.WriteLine();
                    sqlWriter.WriteLine("-- Medical History");
                    foreach (var record in MedicalHistoryCollection)
                    {
                        sqlWriter.WriteLine("INSERT INTO MedicalHistory (StartDate, EndDate, TestResults) VALUES");
                        sqlWriter.WriteLine($"('{record.StartDate:yyyy-MM-dd}', '{record.EndDate:yyyy-MM-dd}', '{record.TestResults}');");
                    }

                    // SQL для записей о лечении
                    sqlWriter.WriteLine();
                    sqlWriter.WriteLine("-- Future Treatment Records");
                    foreach (var record in FutureTreatmentRecords)
                    {
                        sqlWriter.WriteLine("INSERT INTO TreatmentRecords (Date, DoctorFullName) VALUES");
                        sqlWriter.WriteLine($"('{record.Date:yyyy-MM-dd}', '{record.DoctorFullName}');");
                    }
                }

                MessageBox.Show("Данные успешно экспортированы на рабочий стол в файлы 'PatientData.csv' и 'PatientData.sql'.",
                                "Экспорт данных", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}