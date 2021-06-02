using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2.Models
{
    public class ChangePassword
    {
        public string ECNo { get; set; }
        [Required, DataType(DataType.Password), Display(Name ="Current Password")]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword",ErrorMessage ="New Password and Confirm New Password does not match")]
        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
        public string Role { get; set; }
        public string Response { get; set; }
    }
}
