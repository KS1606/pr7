using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class MedicalHistoryPage : Window
    {
        private int historyId;
        private MedicalHistoryTableAdapter medicalHistoryAdapter = new MedicalHistoryTableAdapter();
        private DiagnosesTableAdapter diagnosesAdapter = new DiagnosesTableAdapter();
        private TreatmentRecordTableAdapter treatmentRecordAdapter = new TreatmentRecordTableAdapter();
        private MedicalServicesTableAdapter medicalServicesAdapter = new MedicalServicesTableAdapter();
        private MedicationsTableAdapter medicationsAdapter = new MedicationsTableAdapter();

        public MedicalHistoryModel MedicalHistory { get; set; }

        public MedicalHistoryPage(int historyId)
        {
            InitializeComponent();
            this.historyId = historyId;
            LoadMedicalHistoryData();
            this.DataContext = this;
        }


        private void LoadMedicalHistoryData()
        {
            var medicalHistoryData = medicalHistoryAdapter.GetData().FirstOrDefault(mh => mh.id == historyId);

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



        private List<DiagnosisModel> GetDiagnosesForHistory(int historyId)
        {
            // Получаем диагнозы, связываясь через TreatmentRecord.diagnosis_id
            var diagnoses = diagnosesAdapter.GetData()
                .Where(d => d.id == historyId)  // Диагноз может быть связан с записью через TreatmentRecord
                .Select(d => new DiagnosisModel
                {
                    Name = d.name,
                    Description = d.description,
                    Symptoms = d.symptoms,
                    Treatment = d.treatment
                }).ToList();

            return diagnoses;
        }

        private List<TreatmentRecordModel> GetTreatmentRecordsForHistory(int historyId)
        {
            var treatmentRecords = treatmentRecordAdapter.GetData()
                .Where(tr => tr.history_id == historyId) // Получаем все записи лечения по истории болезни
                .Select(tr => new TreatmentRecordModel
                {
                    Date = tr.date,
                    Description = tr.description,
                    Diagnoses = GetDiagnosesForTreatmentRecord(tr.diagnosis_id),
                    AssignedServices = GetAssignedServicesForTreatmentRecord(tr.id),
                    Prescriptions = GetPrescriptionsForTreatmentRecord(tr.id)
                }).ToList();

            return treatmentRecords;
        }

        private List<DiagnosisModel> GetDiagnosesForTreatmentRecord(int diagnosisId)
        {
            // Получаем диагноз для записи лечения, связываем через diagnosis_id
            var diagnoses = diagnosesAdapter.GetData()
                .Where(d => d.id == diagnosisId)
                .Select(d => new DiagnosisModel
                {
                    Name = d.name,
                    Description = d.description,
                    Symptoms = d.symptoms,
                    Treatment = d.treatment
                }).ToList();

            return diagnoses;
        }

        private List<ServiceModel> GetAssignedServicesForTreatmentRecord(int recordId)
        {
            var services = medicalServicesAdapter.GetData()
                .Where(s => s.id == recordId)  // Получаем услуги, связанные с TreatmentRecord
                .Select(s => new ServiceModel
                {
                    Name = s.name,
                    Description = s.description,
                    Availability = s.availability
                }).ToList();

            return services;
        }

        private List<PrescriptionModel> GetPrescriptionsForTreatmentRecord(int recordId)
        {
            var prescriptions = medicationsAdapter.GetData()
                .Where(m => m.id == recordId)  // Получаем назначения по TreatmentRecord
                .Select(m => new PrescriptionModel
                {
                    MedicationName = m.name,
                    Dosage = m.dosage,
                    Administration = m.administration
                }).ToList();

            return prescriptions;
        }
    }
}
