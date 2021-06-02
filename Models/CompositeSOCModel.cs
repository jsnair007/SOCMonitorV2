using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryDB.Model;

namespace SOCMonitorV2.Models
{
    public class CompositeSOCModel
    {
        public LoginClass Logins { get; set; }
        public List<TaskClass> Tasks { get; set; }
        public List<ShiftSchedule> ShiftSchedules { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Attendance> AttendanceModels { get; set; }
        public ChangePassword ChangePasswordModel { get; set; }
        public string ECNo { get; set; }
        public string Role { get; set; }
        public List<Routine> RoutineComp{ get; set; }
    }
}
