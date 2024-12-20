using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;

namespace MedicalInfo
{
    public partial class DoctorsWindow : Window
    {
        private IEnumerable<Doctor> allDoctors;
        private Doctor _selectedDoctor;

        // Строка подключения к базе данных
        private readonly string connectionString = "Data Source=DESKTOP-KT2TI1B;Initial Catalog=MedicalSystem;Integrated Security=True";

        public DoctorsWindow()
        {
            InitializeComponent();
            LoadDoctors();
        }

        /// <summary>
        /// Загрузка списка врачей из базы данных.
        /// </summary>
        private void LoadDoctors(string filterText = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Фильтрация по имени врача и специализации
                    string query = @"
                        SELECT d.id, d.login_data_id, ld.full_name, s.name AS specialization_name, 
                               d.experience, d.contact_info, d.schedule
                        FROM Doctors d
                        INNER JOIN Specialization s ON d.specialization_id = s.id
                        INNER JOIN LoginData ld ON d.login_data_id = ld.id
                        WHERE (@FilterText = '' OR ld.full_name LIKE @FilterText OR s.name LIKE @FilterText)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FilterText", "%" + filterText + "%");

                    SqlDataReader reader = command.ExecuteReader();

                    var doctorsList = new List<Doctor>();
                    while (reader.Read())
                    {
                        doctorsList.Add(new Doctor
                        {
                            Id = reader.GetInt32(0),
                            login_data_id = reader.GetInt32(1),
                            FullName = reader.GetString(2),
                            SpecializationName = reader.GetString(3),
                            Experience = reader.GetString(4),
                            ContactInfo = reader.GetString(5),
                            Schedule = reader.GetString(6)
                        });
                    }

                    allDoctors = doctorsList;
                    DoctorsListView.ItemsSource = allDoctors.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Удаление врача.
        /// </summary>
        private void DeleteDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDoctor == null)
            {
                MessageBox.Show("Выберите врача для удаления.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Doctors WHERE id = @DoctorId";

                    SqlCommand command = new SqlCommand(deleteQuery, connection);
                    command.Parameters.AddWithValue("@DoctorId", _selectedDoctor.Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Врач успешно удален.");
                        LoadDoctors();  // Перезагружаем список врачей
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить врача.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Изменение данных врача.
        /// </summary>
        private void SaveDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDoctor == null)
            {
                MessageBox.Show("Выберите врача для изменения.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = @"
                        UPDATE Doctors
                        SET experience = @Experience, contact_info = @ContactInfo, 
                            schedule = @Schedule, specialization_id = 
                            (SELECT id FROM Specialization WHERE name = @SpecializationName)
                        WHERE id = @DoctorId";

                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@Experience", ExperienceTextBox.Text);
                    command.Parameters.AddWithValue("@ContactInfo", ContactInfoTextBox.Text);
                    command.Parameters.AddWithValue("@Schedule", ScheduleTextBox.Text);
                    command.Parameters.AddWithValue("@SpecializationName", SpecializationTextBox.Text);
                    command.Parameters.AddWithValue("@DoctorId", _selectedDoctor.Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Изменения успешно сохранены.");
                        LoadDoctors();  // Перезагружаем список врачей
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить изменения.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавление нового врача.
        /// </summary>
        private void AddDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все поля заполнены
            if (string.IsNullOrEmpty(DoctorNameTextBox.Text) || string.IsNullOrEmpty(SpecializationTextBox.Text) ||
                string.IsNullOrEmpty(ExperienceTextBox.Text) || string.IsNullOrEmpty(ContactInfoTextBox.Text) ||
                string.IsNullOrEmpty(ScheduleTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Генерируем случайный логин и пароль для врача
                    string login = GenerateLogin();
                    string password = GeneratePassword();

                    // Проверяем, существует ли уже логин в базе данных
                    string checkLoginQuery = "SELECT COUNT(*) FROM LoginData WHERE login = @Login";
                    SqlCommand checkLoginCommand = new SqlCommand(checkLoginQuery, connection);
                    checkLoginCommand.Parameters.AddWithValue("@Login", login);

                    int existingLoginCount = (int)checkLoginCommand.ExecuteScalar();
                    if (existingLoginCount > 0)
                    {
                        MessageBox.Show("Этот логин уже существует. Попробуйте снова.");
                        return;
                    }

                    // Добавляем данные для входа в таблицу LoginData
                    string insertLoginQuery = "INSERT INTO LoginData (login, password, role, full_name) OUTPUT INSERTED.id VALUES (@Login, @Password, @Role, @FullName)";
                    SqlCommand insertLoginCommand = new SqlCommand(insertLoginQuery, connection);
                    insertLoginCommand.Parameters.AddWithValue("@Login", login);
                    insertLoginCommand.Parameters.AddWithValue("@Password", password);
                    insertLoginCommand.Parameters.AddWithValue("@Role", "Врач");
                    insertLoginCommand.Parameters.AddWithValue("@FullName", DoctorNameTextBox.Text);

                    int loginDataId = (int)insertLoginCommand.ExecuteScalar();

                    // Получаем специализацию врача
                    string getSpecializationQuery = "SELECT id FROM Specialization WHERE name = @SpecializationName";
                    SqlCommand getSpecializationCommand = new SqlCommand(getSpecializationQuery, connection);
                    getSpecializationCommand.Parameters.AddWithValue("@SpecializationName", SpecializationTextBox.Text);
                    int specializationId = (int)getSpecializationCommand.ExecuteScalar();

                    // Добавляем врача в таблицу Doctors
                    string insertDoctorQuery = "INSERT INTO Doctors (login_data_id, experience, contact_info, schedule, specialization_id) VALUES (@LoginDataId, @Experience, @ContactInfo, @Schedule, @SpecializationId)";
                    SqlCommand insertDoctorCommand = new SqlCommand(insertDoctorQuery, connection);
                    insertDoctorCommand.Parameters.AddWithValue("@LoginDataId", loginDataId);
                    insertDoctorCommand.Parameters.AddWithValue("@Experience", ExperienceTextBox.Text);
                    insertDoctorCommand.Parameters.AddWithValue("@ContactInfo", ContactInfoTextBox.Text);
                    insertDoctorCommand.Parameters.AddWithValue("@Schedule", ScheduleTextBox.Text);
                    insertDoctorCommand.Parameters.AddWithValue("@SpecializationId", specializationId);

                    insertDoctorCommand.ExecuteNonQuery();

                    MessageBox.Show("Врач успешно добавлен!");
                    LoadDoctors();  // Перезагружаем список врачей
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Фильтрация списка врачей.
        /// </summary>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string filterText = FilterTextBox.Text;
            LoadDoctors(filterText);  // Перезагружаем список врачей с фильтрацией
        }

        /// <summary>
        /// Выбор врача из списка.
        /// </summary>
        private void DoctorCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Doctor doctor)
            {
                DoctorNameTextBox.Text = doctor.FullName;
                SpecializationTextBox.Text = doctor.SpecializationName;
                ExperienceTextBox.Text = doctor.Experience;
                ContactInfoTextBox.Text = doctor.ContactInfo;
                ScheduleTextBox.Text = doctor.Schedule;
                _selectedDoctor = doctor;
            }
        }

        /// <summary>
        /// Генерация случайного логина.
        /// </summary>
        private string GenerateLogin()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        /// <summary>
        /// Генерация случайного пароля.
        /// </summary>
        private string GeneratePassword()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            return new string(Enumerable.Range(0, 10).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GlavAndinWindow window = new GlavAndinWindow(0);
            Close();
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PreparationsWindow window = new PreparationsWindow();
            Close();
            window.Show();
        }
    }

}
