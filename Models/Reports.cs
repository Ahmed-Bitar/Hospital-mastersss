using System;

namespace MedicalPark.Models
{
    


    public class Report
    {
        public enum ReportType
        {
            PatientReportType = 0,
            StaffReportType,
            ManagementReportType,
            EmergencyReportType,
            VisitorReportType,
        }

        public enum PatientReportType
        {
            None = 0,
            MedicalError = 1,
            PoorService = 2,
            MedicineShortage = 3,
            AppointmentDelay = 4,
            CleanlinessIssues = 5
        }

        public enum StaffReportType
        {
            None = 0,
            WorkplaceIncidents = 1,
            EquipmentIssues = 2,
            StaffShortage = 3
        }

        public enum ManagementReportType
        {
            None = 0,
            InfrastructureProblems,
            DataTampering,
            SecurityViolations
        }

        public enum EmergencyReportType
        {
            None = 0,
            MassInjuries,
            EmergencyShortage
        }

        public enum VisitorReportType
        {
            None = 0,
            VisitorComplaints = 1,
            VisitingHoursIssues = 2
        }
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

         public string UserName { get; set; }
        public ReportType ReportTypes { get; set; }

        public PatientReportType PatientReport { get; set; }
        public StaffReportType StaffReport { get; set; }
        public ManagementReportType ManagementReport { get; set; } 
        public EmergencyReportType EmergencyReport { get; set; }
        public VisitorReportType VisitorReport { get; set; }

        public int UserId { get; set; }



    }
}
