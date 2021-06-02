using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//this class is for making dropdown list. Data should be inserted automatically while adding new employee
namespace SOCMonitorV2.Models
{
    public class OfficialsModel 
    {
        [Key]
        public string ECNo { get; set; }
        public string Name { get; set; }
    }
}
