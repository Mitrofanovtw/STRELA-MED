using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRELA_MED.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Message { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
