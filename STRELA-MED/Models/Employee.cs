using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRELA_MED.Models
{
    [Table("Employees")]
    public class Employee
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "User";

        [Column("Id")]
        public int Id { get; set; }
        [Column("LastName")]
        public string LastName { get; set; }
        [Column("FirstName")]
        public string FirstName { get; set; }
        [Column("MiddleName")]
        public string? MiddleName { get; set; }

        [Column("BirthDate")]
        public DateTime BirthDate { get; set; }
        [Column("Position")]
        public string Position { get; set; }
        [Column("ChronicDiseases")]
        public string? ChronicDiseases { get; set; }
        [Column("MedicalHistory")]
        public string? MedicalHistory { get; set; }
        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("FullName")]
        public string FullName => $"{LastName} {FirstName} {MiddleName}";
        [Column("BirthDateDisplay")]
        public string BirthDateDisplay => BirthDate.ToString("dd.MM.yyyy");
    }
}
