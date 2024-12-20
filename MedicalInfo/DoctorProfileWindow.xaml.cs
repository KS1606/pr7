using System;
using System.ComponentModel; // Для INotifyPropertyChanged
using MedicalInfo.MedicalSystemDataSetTableAdapters;
using System.Windows;
using System.Linq;

namespace MedicalInfo
{
    public partial class DoctorProfileWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int userId;
        private LoginDataTableAdapter loginDataAdapter = new LoginDataTableAdapter(); // Адаптер для таблицы LoginData
        private DoctorsTableAdapter doctorsDataAdapter = new DoctorsTableAdapter(); // Адаптер для таблицы Doctors
        private SpecializationTableAdapter specializationDataAdapter = new SpecializationTableAdapter(); // Адаптер для таблицы Specialization

        // Свойства для привязки
        private string fullName;
        private string experience;
        private string contactInfo;
        private string schedule;

        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Experience
        {
            get => experience;
            set
            {
                experience = value;
                OnPropertyChanged(nameof(Experience));
            }
        }

        public string ContactInfo
        {
            get => contactInfo;
            set
            {
                contactInfo = value;
                OnPropertyChanged(nameof(ContactInfo));
            }
        }

        public string Schedule
        {
            get => schedule;
            set
            {
                schedule = value;
                OnPropertyChanged(nameof(Schedule));
            }
        }

        public DoctorProfileWindow(int UserId)
        {
            InitializeComponent();
            this.userId = UserId;
            this.DataContext = this;  // Устанавливаем DataContext в саму себя
            LoadDoctorData();
        }

        private void LoadDoctorData()
        {
            try
            {
                // Получаем полное имя из таблицы LoginData
                var loginData = loginDataAdapter.GetData()
                    .AsEnumerable()  // Преобразуем в коллекцию, поддерживающую LINQ
                    .FirstOrDefault(ld => ld.id == userId); // Поиск по id из LoginData

                if (loginData != null)
                {
                    FullName = loginData.full_name;
                }
                else
                {
                    MessageBox.Show("Доктор не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем данные из таблицы Doctors по login_data_id
                var doctorData = doctorsDataAdapter.GetData()
                    .AsEnumerable()  // Преобразуем в коллекцию, поддерживающую LINQ
                    .FirstOrDefault(d => d.login_data_id == userId); // Поиск по login_data_id

                if (doctorData != null)
                {
                    Experience = doctorData.experience;

                    // Присваиваем contact_info как есть, без разделения
                    ContactInfo = doctorData.contact_info;

                    Schedule = doctorData.schedule;
                }
                else
                {
                    MessageBox.Show("Нет данных о враче!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
