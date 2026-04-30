using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRELA_MED.Models
{
    public class EmployeeExamStatus
    {
        public Employee Employee { get; set; }
        public string FullName => $"{Employee.LastName} {Employee.FirstName} {Employee.MiddleName}";
        public string LastExamDate { get; set; }
    }
}
