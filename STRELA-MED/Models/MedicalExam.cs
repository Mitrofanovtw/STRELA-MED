using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STRELA_MED.Models
{
    public class MedicalExam
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        public DateTime ExamDate { get; set; }
        public string ExamType { get; set; }
        public string DoctorName { get; set; }
        public string Result { get; set; }
        public DateTime ValidUntil { get; set; }

        public string? BloodPressure { get; set; }
        public int? Pulse { get; set; }
        public string? Complaints { get; set; }
        public string? Conclusion { get; set; }
        public double? AlcoholLevel { get; set; }
    }
}