using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using SOCMonitorV2.Models;
using ClassLibraryDB.DAL;
using ClassLibraryDB.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace SOCMonitorV2.Controllers
{
    public class ShiftController : Controller
    {
        private static IConfiguration _iconfiguration;
        const string SessionName = "_Name";
        const string SessionRole = "_Role";
        private readonly ShiftContext _Db;

    public ShiftController(ShiftContext Db)
        {
            _Db = Db;
        }

        public IActionResult SelectRosterMonth()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public IActionResult ShiftList(ShiftModel objS)
        {
            try
            {
                ViewBag.Role= HttpContext.Session.GetString(SessionRole);
                ViewBag.Name = HttpContext.Session.GetString(SessionName);
                string sql = "select * from tbl_Shift where DutyStartTime is not NULL order by DutyDate ";
                //var param1 = new SqlParameter("currMonth", "%12%");
                //var shList = _Db.tbl_Shift.FromSqlRaw(sql, param1).ToList();
                var shList = _Db.tbl_Shift.FromSqlRaw(sql).ToList();
                //var shList = _Db.tbl_Shift.ToList();
                return View(shList);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public IActionResult Create(ShiftModel objS)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            //loadDDL();
            if(objS.ID!=0)
            {
                loadDDL(objS.ECNo, objS.Name);
            }
            else
            {
                loadDDL("","");
            }
            return View(objS);
        }
        [HttpPost]
        public async Task<IActionResult> AddShiftDuty(ShiftModel objS)
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            var name = _Db.tbl_Officials.ToList();
            string DutyDate = string.Empty;
            for(int i=0;i<name.Count;i++)
            {
                if(name[i].ECNo==objS.ECNo)
                {
                    objS.Name = name[i].Name;
                }
            }

            try
            {
                
                switch (objS.ShiftTimeBase)
                {
                    case "Morning":
                        //TimeSpan ts = new TimeSpan(07, 00, 00);
                        DutyDate = objS.DutyDate.ToString("yyyy-MM-dd ");
                        objS.DutyStartTime = Convert.ToDateTime(DutyDate + new TimeSpan(07, 30, 00));
                        objS.DutyEndTime = Convert.ToDateTime(objS.DutyStartTime).AddHours(8);
                        break;
                    case "General":
                        DutyDate = objS.DutyDate.ToString("yyyy-MM-dd ");
                        objS.DutyStartTime = Convert.ToDateTime(DutyDate + new TimeSpan(10, 00, 00));
                        objS.DutyEndTime = Convert.ToDateTime(objS.DutyStartTime).AddHours(7); 
                        break;
                    case "Afternoon":
                        DutyDate = objS.DutyDate.ToString("yyyy-MM-dd ");
                        objS.DutyStartTime = Convert.ToDateTime(DutyDate + new TimeSpan(15, 00, 00)); 
                        objS.DutyEndTime = Convert.ToDateTime(objS.DutyStartTime).AddHours(8); 
                        break;
                    case "Night":
                        DutyDate = objS.DutyDate.ToString("yyyy-MM-dd ");
                        objS.DutyStartTime = Convert.ToDateTime(DutyDate + new TimeSpan(23, 00, 00)); 
                        objS.DutyEndTime = Convert.ToDateTime(objS.DutyStartTime).AddHours(8.50); 
                        break;
                    default:
                        break;
                }
                if (objS.ID == 0)
                {
                    _Db.tbl_Shift.Add(objS);
                    await _Db.SaveChangesAsync();
                }
                else
                {
                    _Db.Entry(objS).State = EntityState.Modified;
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("ShiftList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ShiftList");
            }
        }
        
        public async Task<IActionResult> DeleteShiftDuty(int id)
        {
            try
            {
                ViewBag.Name= HttpContext.Session.GetString(SessionName);
                var sh = await _Db.tbl_Shift.FindAsync(id);
                if (sh != null)
                {
                    _Db.tbl_Shift.Remove(sh);
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("ShiftList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ShiftList");
            }
        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }
        
        static List<Attendance> GetAttendanceDetails(string Name)
        {
            var AttendanceDAL = new AttendanceDAL(_iconfiguration);
            List<Attendance> attendanceList = AttendanceDAL.GetShiftScheduleDetailsForAttendance(Name);
            List<Attendance> objAL = attendanceList;
            return objAL;
        }


        private void loadDDL(string id, string value)
        {
            try
            {
                List<OfficialsModel> offList = new List<OfficialsModel>();
                offList = _Db.tbl_Officials.ToList();
                if (id == string.Empty)
                    offList.Insert(0, new OfficialsModel { ECNo = "", Name = "-- Select Duty Officer --" });
                else
                    offList.Insert(0, new OfficialsModel { ECNo = id, Name = value });
                //offList.Insert(0, new OfficialsModel { ECNo = "", Name = "-- Select Duty Officer --" });
                ViewBag.DepList = offList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
