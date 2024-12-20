using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class EditMedicationWindow : Window
    {
        private MedicationModel _medication;

        public EditMedicationWindow(MedicationModel medication)
        {
            InitializeComponent();

            _medication = medication;

            // Заполняем поля данными препарата
            NameTextBox.Text = _medication.Name;
            DosageTextBox.Text = _medication.Dosage;
            AdministrationTextBox.Text = _medication.Administration;

            // Устанавливаем выбранную форму
            var formItem = FormComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _medication.DosageForm);
            if (formItem != null)
            {
                FormComboBox.SelectedItem = formItem;
            }
            _medication = medication;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем изменения
            _medication.Name = NameTextBox.Text;
            _medication.Dosage = DosageTextBox.Text;
            _medication.Administration = AdministrationTextBox.Text;
            _medication.DosageForm = (FormComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Логика сохранения изменений в базе данных
            SaveMedicationToDatabase(_medication);

            // Закрываем окно и возвращаем результат
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Просто закрыть окно без сохранения
            DialogResult = false;
            Close();
        }

        private void SaveMedicationToDatabase(MedicationModel medication)
        {
            // Логика сохранения изменений в базе данных
            try
            {
                using (var connection = new SqlConnection("Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True"))
                {
                    connection.Open();

                    string query = "UPDATE Medications SET name = @Name, dosage = @Dosage, administration = @Administration, form_id = @FormId WHERE name = @OldName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", medication.Name);
                        command.Parameters.AddWithValue("@Dosage", medication.Dosage);
                        command.Parameters.AddWithValue("@Administration", medication.Administration);
                        command.Parameters.AddWithValue("@FormId", GetFormIdByName(medication.DosageForm));
                        command.Parameters.AddWithValue("@OldName", medication.Name); // Используем старое имя для обновления

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetFormIdByName(string formName)
        {
            // Логика получения ID формы по имени (нужно настроить, как это делать в вашей базе данных)
            switch (formName)
            {
                case "Таблетки":
                    return 1;
                case "Капсулы":
                    return 2;
                case "Сироп":
                    return 3;
                case "Инъекции":
                    return 4;
                case "Мазь":
                    return 5;
                default:
                    return 0;
            }
        }
    }
}
