using System;
using System.ComponentModel.DataAnnotations;
using MedicalPark.Models;

namespace MedicalPark.Dtos
{
    public class SurgicalOperationDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime OperationStartTime { get; set; }

      
        public int DurationInMinutes { get; set; }

        public string? Description { get; set; }

        public int CostOfOperation { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int NurseId { get; set; }

        public int RoomId { get; set; }

    }
}
