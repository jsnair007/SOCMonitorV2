using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2.Models
{
    public class ExecutiveModel
    {
        [Key]
        public string ECNo { get; set; }
        public string Name { get; set; }
    }
}
