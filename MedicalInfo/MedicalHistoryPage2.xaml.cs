using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class MedicalHistoryPage2 : Window
    {
        private readonly int _historyId;
        private readonly MedicalHistoryTableAdapter _medicalHistoryAdapter = new MedicalHistoryTableAdapter();
        private readonly DiagnosesTableAdapter _diagnosesAdapter = new DiagnosesTableAdapter();
        private readonly TreatmentRecordTableAdapter _treatmentRecordAdapter = new TreatmentRecordTableAdapter();
        private readonly MedicalServicesTableAdapter _medicalServicesAdapter = new MedicalServicesTableAdapter();
        private readonly MedicationsTableAdapter _medicationsAdapter = new MedicationsTableAdapter();
        private readonly AssignedServicesTableAdapter _assignedServicesAdapter = new AssignedServicesTableAdapter();
        private readonly PrescriptionsTableAdapter _prescriptionsAdapter = new PrescriptionsTableAdapter();

        public MedicalHistoryModel MedicalHistory { get; set; }

        public MedicalHistoryPage2(int historyId)
        {
            InitializeComponent();
            _historyId = historyId;
            LoadMedicalHistoryData();
            this.DataContext = this;
        }

        private void LoadMedicalHistoryData()
        {
            try
            {
                var medicalHistoryData = _medicalHistoryAdapter.GetData().FirstOrDefault(mh => mh.id == _historyId);

                if (medicalHistoryData != null)
                {
                    MedicalHistory = new MedicalHistoryModel
                    {
                        StartDate = medicalHistoryData.start_date,
                        EndDate = medicalHistoryData.end_date,
                        TestResults = medicalHistoryData.test_results,
                        Diagnoses = GetDiagnosesForHistory(medicalHistoryData.id),
                        TreatmentRecords = GetTreatmentRecordsForHistory(medicalHistoryData.id)
                    };

                    DataContext = MedicalHistory;
                }
                else
                {
                    MessageBox.Show("Данные не найдены для данной истории болезни.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных истории болезни: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<DiagnosisModel> GetDiagnosesForHistory(int historyId)
        {
            try
            {
                var diagnoses = _diagnosesAdapter.GetData()
                    .Join(_treatmentRecordAdapter.GetData(),
                          diagnosis => diagnosis.id,
                          treatmentRecord => treatmentRecord.diagnosis_id,
                          (diagnosis, treatmentRecord) => new { diagnosis, treatmentRecord })
                    .Where(x => x.treatmentRecord.history_id == historyId)
                    .Select(x => new DiagnosisModel
                    {
                        Name = x.diagnosis.name,
                        Description = x.diagnosis.description,
                        Symptoms = x.diagnosis.symptoms,
                        Treatment = x.diagnosis.treatment
                    }).ToList();

                return diagnoses ?? new List<DiagnosisModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке диагнозов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<DiagnosisModel>();
            }
        }

        private List<TreatmentRecordModel> GetTreatmentRecordsForHistory(int historyId)
        {
            try
            {
                var treatmentRecords = _treatmentRecordAdapter.GetData()
                    .Where(tr => tr.history_id == historyId)
                    .Select(tr => new TreatmentRecordModel
                    {
                        Id = tr.id,
                        Date = tr.date,
                        Description = tr.description,
                        Diagnoses = GetDiagnosesForHistory(historyId),
                        AssignedServices = GetAssignedServicesForTreatmentRecord(tr.id),
                        Prescriptions = GetPrescriptionsForTreatmentRecord(tr.id)
                    }).ToList();

                return treatmentRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке записей лечения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<TreatmentRecordModel>();
            }
        }

        private List<ServiceModel> GetAssignedServicesForTreatmentRecord(int recordId)
        {
            try
            {
                var services = _medicalServicesAdapter.GetData()
                    .Join(_assignedServicesAdapter.GetData(),
                          service => service.id,
                          assignedService => assignedService.service_id,
                          (service, assignedService) => new { service, assignedService })
                    .Where(x => x.assignedService.record_id == recordId)
                    .Select(x => new ServiceModel
                    {
                        Name = x.service.name,
                        Description = x.service.description,
                        Availability = x.service.availability
                    }).ToList();

                return services ?? new List<ServiceModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке услуг: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<ServiceModel>();
            }
        }

        private List<PrescriptionModel> GetPrescriptionsForTreatmentRecord(int recordId)
        {
            try
            {
                var prescriptions = _medicationsAdapter.GetData()
                    .Join(_prescriptionsAdapter.GetData(),
                          medication => medication.id,
                          prescription => prescription.medication_id,
                          (medication, prescription) => new { medication, prescription })
                    .Where(x => x.prescription.record_id == recordId)
                    .Select(x => new PrescriptionModel
                    {
                        MedicationName = x.medication.name,
                        Dosage = x.medication.dosage,
                        Administration = x.medication.administration
                    }).ToList();

                return prescriptions ?? new List<PrescriptionModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке назначений препаратов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<PrescriptionModel>();
            }
        }

        private void EditTreatmentRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.CommandParameter is TreatmentRecordModel treatmentRecord)
                {
                    var editWindow = new EditTreatmentRecordWindow(treatmentRecord);
                    bool? result = editWindow.ShowDialog();

                    if (result == true)
                    {
                        LoadMedicalHistoryData(); 
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить запись для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем данные в базе данных с помощью адаптера для истории болезни
                var medicalHistoryData = _medicalHistoryAdapter.GetData().FirstOrDefault(mh => mh.id == _historyId);

                if (medicalHistoryData != null)
                {
                    // Обновляем значения полей истории болезни
                    medicalHistoryData.start_date = MedicalHistory.StartDate;
                    medicalHistoryData.end_date = MedicalHistory.EndDate;
                    medicalHistoryData.test_results = MedicalHistory.TestResults;

                    // Сохраняем изменения в базе данных
                    _medicalHistoryAdapter.Update(medicalHistoryData);

                    // Сохраняем изменения для записей лечения
                    foreach (var treatmentRecord in MedicalHistory.TreatmentRecords)
                    {
                        // Обновляем запись лечения в базе данных
                        var treatmentRecordData = _treatmentRecordAdapter.GetData().FirstOrDefault(tr => tr.id == treatmentRecord.Id);
                        if (treatmentRecordData != null)
                        {
                            treatmentRecordData.date = treatmentRecord.Date;
                            treatmentRecordData.description = treatmentRecord.Description;
                            _treatmentRecordAdapter.Update(treatmentRecordData);

                        }
                    }

                    
                    MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось найти историю болезни для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибки, если они возникнут
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
