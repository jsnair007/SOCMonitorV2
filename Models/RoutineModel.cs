using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.EntityClient;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOCMonitorV2.Models
{
    public class RoutineModel
    {
        public int ID { get; set; }
        public string TaskId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        [Display(Name = "Subject")]
        public string ShortDescription { get; set; }

        [Required]
        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }
        public string CreatedDate { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        [Display(Name = "Execution Date")] public DateTime ExecutionDate { get; set; }
        public string Frequency { get; set; }
        [Display(Name = "Task Status")] public string CompletedFlg { get; set; }
        public string Priority { get; set; }
        public string ClosureDate { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Exec. Remarks")] public string ExecRemarks { get; set; }
        
    }
}
