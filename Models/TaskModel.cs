using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SOCMonitorV2.Models
{
    public class TaskModel
    {
        [Key]
        public int ID { get; set; }

        
        [Required][StringLength(20, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        [Display(Name = "Subject")]
        public string ShortDescription { get; set; }

        [Required][Display(Name = "Full Description")]
        public string FullDescription { get; set; }
        public string CreatedDate { get; set; }
        public string CreatorId { get; set; }
        [Display(Name = "Creator")] public string CreatorName { get; set; }
        public string AssignedBy { get; set; }
        [Required][Display(Name ="Assigned To/ Pending With")]
        public string AssignTo { get; set; }

        [Display(Name = "Assign Date")]
        public string AssignDate { get; set; }

        
        [Required][Display(Name = "Exp.Closure Time")] 
        public string ETC { get; set; }
        
        
        [Display(Name = "Status")]
        public string TaskStatus { get; set; }
        
        
        public string Priority { get; set; }

        [Display(Name = "Closure Date")] public string ClosureDate { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Exec.Comments")]
        public string ExecutivesComments { get; set; }
    }
}
