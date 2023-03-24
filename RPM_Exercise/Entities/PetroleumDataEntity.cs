using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPM_Exercise.Entities
{
    [Table("PriceHistory", Schema = "Petroleum")]
    public class PetroleumDataEntity
    {
        [Key]
        public DateOnly Period { get; set; }
        public double Price { get; set; }
    }
}
