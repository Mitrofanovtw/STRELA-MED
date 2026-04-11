using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRELA_MED.Models
{
    [Table("medicine")]
    public class Medicine
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("unit")]
        public string Unit { get; set; }
        [Column("expiration_date")]
        public string ExpirationDate { get; set; }
    }
}
