using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SOCMonitorV2.Models
{
    public class RoutineContext : DbContext
    {
        public RoutineContext(DbContextOptions<RoutineContext> options) : base(options)
        {

        }
        public DbSet<RoutineModel> tbl_Routine { get; set; }
        //public DbSet<OfficialsModel> tbl_Officials { get; set; }
        //public DbSet<EmployeeModel> tbl_Employee { get; set; }
    }
}
