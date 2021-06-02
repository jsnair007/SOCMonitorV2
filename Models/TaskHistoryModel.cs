using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SOCMonitorV2.Models
{
    public class TaskHistoryModel
    {
        [Key]
        public int ID { get; set; }
        public int Task_ID { get; set; }
        [Display(Name = "Description")]
        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }
        public string CreatedDate { get; set; }
        public string CreatorId { get; set; }
        [Display(Name = "Creator")] public string CreatorName { get; set; }
        public string AssignedBy { get; set; }
        [Display(Name ="Pending With")] public string AssignTo { get; set; }

        public string AssignDate { get; set; }

        [Display(Name = "Exp.Closure Time")]
        public string ETC { get; set; }

        [Display(Name = "Status")]
        public string TaskStatus { get; set; }

        public string Priority { get; set; }

        [Display(Name = "Closure Date")] public string ClosureDate { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Exec.Comments")]
        public string ExecutivesComments { get; set; }
        [Display(Name = "Modified Time")] public DateTime R_Mod_Time { get; set; }
        public string ModifiedBy { get; set; }
    }
}
