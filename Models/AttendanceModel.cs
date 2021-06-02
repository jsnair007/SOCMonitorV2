using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2.Models
{
    public class AttendanceModel
    {
        public string ECNo { get; set; }
        public string Name { get; set; }
        //public DateTime CheckinDate { get; set; }
        public string DutyDate { get; set; }
        public string CheckinDateTime { get; set; }
        public string IsPresent { get; set; }

        //public string ShiftTimeBase { get; set; }
        //public string DutyStartTime { get; set; }
        //public string DutyEndTime { get; set; }
    }
}
