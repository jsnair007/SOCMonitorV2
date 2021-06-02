using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SOCMonitorV2.Models
{
    public class ShiftContext : DbContext
    {

        public ShiftContext(DbContextOptions<ShiftContext> options) : base(options)
        {

        }
        public DbSet<ShiftModel> tbl_Shift { get; set; }
        public DbSet<OfficialsModel> tbl_Officials { get; set; }
    }
}
