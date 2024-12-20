using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls;

namespace MedicalInfo
{
    public partial class PreparationsWindow : Window
    {
        // Коллекция для хранения препаратов
        public ObservableCollection<MedicationModel> Medications { get; set; }

        private MedicationsTableAdapter medicationsAdapter = new MedicationsTableAdapter();
        private DosageFormTableAdapter dosageFormAdapter = new DosageFormTableAdapter();

        // Команда для удаления препарата
        public RelayCommand DeleteMedicationCommand { get; private set; }

        public PreparationsWindow()
        {
            InitializeComponent();

            // Инициализация команды редактирования
            EditMedicationCommand = new RelayCommand(EditMedication);

            // Инициализация команды удаления
            DeleteMedicationCommand = new RelayCommand(DeleteMedication);

            // Привязка данных к окну
            DataContext = this;

            // Загрузка данных о препаратах
            LoadMedicationsData();
        }

        private void LoadMedicationsData()
        {
            try
            {
                // Инициализируем коллекцию
                Medications = new ObservableCollection<MedicationModel>();

                // Получаем данные о препаратах
                var medicationsData = medicationsAdapter.GetData();
                var dosageFormsData = dosageFormAdapter.GetData();

                // Заполняем коллекцию
                foreach (var medication in medicationsData)
                {
                    var dosageForm = dosageFormsData.FirstOrDefault(df => df.id == medication.form_id);
                    Medications.Add(new MedicationModel
                    {
                        Name = medication.name,
                        Dosage = medication.dosage,
                        Administration = medication.administration,
                        DosageForm = dosageForm?.name ?? "Не указано"
                    });
                }

                // Устанавливаем источник данных для ItemsControl
                MedicationsItemsControl.ItemsSource = Medications;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о препаратах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Событие для кнопки, если нужно
        }

        private void ImportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            // Выбор CSV-файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Выберите файл CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    ImportCsvDataToDatabase(filePath);
                    MessageBox.Show("Данные успешно импортированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновление отображения препаратов
                    LoadMedicationsData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportCsvDataToDatabase(string filePath)
        {
            // Строка подключения к базе данных
            string connectionString = "Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Чтение CSV-файла
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split(',');

                        if (fields.Length < 4)
                            throw new Exception("Неверный формат CSV. Ожидались столбцы: Name, Dosage, Administration, FormId.");

                        string name = fields[0].Trim();
                        string dosage = fields[1].Trim();
                        string administration = fields[2].Trim();
                        int formId = int.Parse(fields[3].Trim());

                        // SQL-запрос для вставки данных
                        string query = @"
                            INSERT INTO Medications (name, dosage, administration, form_id) 
                            VALUES (@Name, @Dosage, @Administration, @FormId)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@Dosage", dosage);
                            command.Parameters.AddWithValue("@Administration", administration);
                            command.Parameters.AddWithValue("@FormId", formId);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedForm = ((ComboBoxItem)FormComboBox.SelectedItem)?.Content.ToString();

            if (selectedForm != null)
            {
                // Фильтруем по выбранной форме
                var filteredMedications = Medications.Where(m => m.DosageForm == selectedForm).ToList();

                // Обновляем ItemsSource
                MedicationsItemsControl.ItemsSource = new ObservableCollection<MedicationModel>(filteredMedications);
            }
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем фильтрацию и возвращаем все препараты
            MedicationsItemsControl.ItemsSource = Medications;
            FormComboBox.SelectedIndex = 0; // Сбросить выбор формы
        }

        private void DeleteMedication(object parameter)
        {
            if (parameter is MedicationModel medication)
            {
                // Удаляем препарат из базы данных
                try
                {
                    using (var connection = new SqlConnection("Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True"))
                    {
                        connection.Open();

                        string query = "DELETE FROM Medications WHERE name = @Name AND dosage = @Dosage AND administration = @Administration";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", medication.Name);
                            command.Parameters.AddWithValue("@Dosage", medication.Dosage);
                            command.Parameters.AddWithValue("@Administration", medication.Administration);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Удаляем препарат из коллекции
                    Medications.Remove(medication);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления препарата: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportSqlButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалогового окна для выбора SQL файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "SQL Files (*.sql)|*.sql",
                Title = "Выберите файл SQL"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    ImportSqlData(filePath);
                    MessageBox.Show("Данные успешно импортированы из SQL!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновление отображения препаратов
                    LoadMedicationsData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта данных из SQL: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportSqlData(string filePath)
        {
            // Чтение SQL-файла с указанием кодировки UTF-8
            string sqlQuery;
            try
            {
                // Указываем кодировку UTF-8 для правильного чтения русских символов
                sqlQuery = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Строка подключения к базе данных
            string connectionString = "Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Разделение SQL-запросов, если их несколько в файле
                    string[] queries = sqlQuery.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var query in queries)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // Выводим сообщение об успешном импорте
                MessageBox.Show("SQL-данные успешно импортированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта данных из SQL: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public RelayCommand EditMedicationCommand { get; private set; }


        private void EditMedication(object parameter)
        {
            if (parameter is MedicationModel medication)
            {
                // Открываем окно редактирования
                var editWindow = new EditMedicationWindow(medication);
                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем данные в коллекции, если они изменены
                    LoadMedicationsData();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoctorsWindow window = new DoctorsWindow();
            Close();
            window.Show();
        }
    }

    // Команда для привязки кнопки удаления
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
