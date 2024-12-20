using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInfo
{

    public class MedicalHistoryModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TestResults { get; set; }
        public List<DiagnosisModel> Diagnoses { get; set; }
        public List<TreatmentRecordModel> TreatmentRecords { get; set; }
    }

    public class DiagnosisModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Symptoms { get; set; }
        public string Treatment { get; set; }
    }

    public class TreatmentRecordModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public List<ServiceModel> AssignedServices { get; set; }
        public List<PrescriptionModel> Prescriptions { get; set; }
        public List<DiagnosisModel> Diagnoses { get; internal set; }
        public int HistoryId { get; set; } // ID истории болезни
        public int DiagnosisId { get; set; } // ID диагноза
        public int DoctorId { get; set; } // ID врача
        public string DoctorFullName { get; set; }
    }

    public class DoctorModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    public class ServiceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Availability { get; set; }
    }

    public class PrescriptionModel
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Administration { get; set; }
    }

    public class MedicationModel
    {
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Administration { get; set; }
        public string DosageForm { get; set; } 
    }

    public class Doctor
    {
        public int Id { get; set; }
        public int login_data_id { get; set; }
        public string FullName { get; set; }
        public string SpecializationName { get; set; }
        public string Experience { get; set; }
        public string ContactInfo { get; set; }
        public string Schedule { get; set; }
    }

    public class Patient
    {
        public int Id { get; set; } // Уникальный идентификатор пациента
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
    }

}
