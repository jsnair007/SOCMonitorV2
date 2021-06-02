using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SOCMonitorV2.Models
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {

        }
        public DbSet<TaskModel> tbl_Task { get; set; }
        public DbSet<OfficialsModel> tbl_Officials { get; set; }
        public DbSet<EmployeeModel> tbl_Employee { get; set; }
        public DbSet<TaskHistoryModel> tbl_Task_History { get; set; }
    }
}
