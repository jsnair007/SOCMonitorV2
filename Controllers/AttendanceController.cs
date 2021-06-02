using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryDB.DAL;
using ClassLibraryDB.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SOCMonitorV2.Models;

namespace SOCMonitorV2.Controllers
{
    public class AttendanceController : Controller
    {
        const string SessionRole = "_Role";
        const string SessionName = "_Name";
        private static IConfiguration _iconfiguration;
        public IActionResult AttendanceList()
        {
            string attendanceMonth = "All";
            try
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionName);
                ViewBag.Role = HttpContext.Session.GetString(SessionRole);
                GetAppSettingsFile();
                List<Attendance> objList = GetAttendanceList(attendanceMonth);
                return View(objList);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        static void GetAppSettingsFile()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                     .SetBasePath(Directory.GetCurrentDirectory())
                                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                _iconfiguration = builder.Build();
            }
            catch (Exception ee)
            {
                clsEvent.LogEvent("SQL Connection exception: ", ee.Message);
            }
        }
        static List<Attendance> GetAttendanceList(string attendanceMonth)
        {
            var AttendanceDAL = new AttendanceDAL(_iconfiguration);
            List<Attendance> attendanceList = AttendanceDAL.GetAttendanceList(attendanceMonth);
            List<Attendance> objAL = attendanceList;
            return objAL;
        }

    }
}
