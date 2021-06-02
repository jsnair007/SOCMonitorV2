using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SOCMonitorV2.Models
{
    public class LoginModel
    {
        public string ECNo { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Organisation { get; set; }
        public string Role { get; set; }
        public string ResponseCode { get; set; }
        public string SessionId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string IsExecutive { get; set; }
        public string ReportingAuth { get; set; }
    }
}
