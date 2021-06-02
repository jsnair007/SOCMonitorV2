using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SOCMonitorV2.Models
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }
        public DbSet<EmployeeModel> tbl_Employee { get; set; }
        public DbSet<ExecutiveModel> tbl_Executives { get; set; }
        public DbSet<OfficialsModel> tbl_Officials { get; set; }
        

    }
}
