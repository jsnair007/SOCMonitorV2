using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2.Models
{
    public class ShiftModel
    {
        [Key]
        public int ID { get; set; }
        public string ECNo { get; set; }
        public string Name { get; set; }

        public DateTime DutyDate { get; set; }

        public string ShiftTimeBase { get; set; }
        public DateTime DutyStartTime { get; set; }
        public DateTime DutyEndTime { get; set; }
        public string Remarks { get; set; }
        
    }
}
