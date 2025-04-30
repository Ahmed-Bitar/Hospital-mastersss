using System;
using static MedicalPark.Models.Report;

namespace MedicalPark.Models
{
    public class ReportDto
    {


        public string Description { get; set; }

        public ReportType ReportTypes { get; set; }
        public PatientReportType PatientReport { get; set; }
        public StaffReportType StaffReport { get; set; }
        public ManagementReportType ManagementReport { get; set; }
        public EmergencyReportType EmergencyReport { get; set; }
        public VisitorReportType VisitorReport { get; set; }
        public DateTime CreatedDate { get; set; }


        public string ReportedBy { get; set; }

    }
}
