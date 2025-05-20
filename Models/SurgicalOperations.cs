using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalPark.Models
{
   
    public class SurgicalOperation
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime OperationStartTime { get; set; }

        [Required]
        public int DurationInMinutes { get; set; }

        public string CreatedByAdminName { get; set; }

        public string Description { get; set; }

        [Required]
        public int CostOfOperation { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public int NurseId { get; set; }
        public Nurse Nurse { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        [Required]

        public string PatientName { get; set; }
        [Required]


        public string DoctorName { get; set; }
        [Required]

        public string NurseName { get; set; }
        [Required]

        public string RoomName { get; set; }
        [Required]


        public bool IsPatientDischarged { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        [Required]
        public ICollection<PatientConditionAfterSurgery> PatientConditions { get; set; } = new List<PatientConditionAfterSurgery>();

    }
}