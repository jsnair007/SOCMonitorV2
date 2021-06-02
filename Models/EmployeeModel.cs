using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SOCMonitorV2.Models
{
    public class EmployeeModel
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName="varchar(50)")]
        [Required]
        public string ECNo { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string Designation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Role { get; set; }

        public bool IsExecutive { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string Location { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string Organisation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ReportingAuth { get; set; }

        [Column(TypeName = "varchar(10)")]
        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Wrong mobile!")]
        [StringLength(10)]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
    }
}
