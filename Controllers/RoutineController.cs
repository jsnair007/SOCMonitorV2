using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOCMonitorV2.Models;
using ClassLibraryDB.DAL;
using ClassLibraryDB.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
namespace SOCMonitorV2.Controllers
{
    public class RoutineController : Controller
    {
        private readonly RoutineContext _Db;
        const string SessionName = "_Name";
        const string SessionDesignation = "_Designation";
        const string SessionRole = "_Role";
        const string SessionECNo = "_ECNo";
        const string SessionAssignTo = "_AssignTo";
        const string SessionErrorMessage = "_ErrorMessage";
        private static IConfiguration _iconfiguration;

        public static object AutoMapper { get; private set; }

        public RoutineController(RoutineContext Db)
        {
            _Db = Db;
        }

        public IActionResult RoutineList()
        {
            string role = HttpContext.Session.GetString(SessionRole);
            string desig = HttpContext.Session.GetString(SessionDesignation);
            string ecno = HttpContext.Session.GetString(SessionECNo);
            ViewBag.Role = role;
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.ECNo = ecno;
            try
            {
                CompositeSOCModel comp = new CompositeSOCModel();
                GetAppSettingsFile();
                string StartD = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string EndD = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                List<Routine> objTaskList = GetRoutineTaskDetails(StartD, EndD);
                comp.RoutineComp = objTaskList;
                return View(comp);
            }
            catch (Exception e)
            {
                return View();
            }
        }
        static List<ClassLibraryDB.Model.Routine> GetRoutineTaskDetails(string StartDate, string EndDate)
        {
            var RoutineTaskDAL = new RoutineTaskDAL(_iconfiguration);
            var listTask = RoutineTaskDAL.GetRoutineList(StartDate, EndDate);
            List<ClassLibraryDB.Model.Routine> objTaskList = new List<ClassLibraryDB.Model.Routine>();
            objTaskList = listTask;
            return objTaskList;
        }
        static List<ClassLibraryDB.Model.Routine> GetRoutineHistoryDetails(string ID)
        {
            var RoutineTaskDAL = new RoutineTaskDAL(_iconfiguration);
            var listTask = RoutineTaskDAL.GetRoutineHistoryList(ID);
            List<ClassLibraryDB.Model.Routine> objTaskList = new List<ClassLibraryDB.Model.Routine>();
            objTaskList = listTask;
            return objTaskList;
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
        public IActionResult Create(Models.RoutineModel objR)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            return View(objR);
        }
        static bool InsertRoutineTask(RoutineModel objRoutine)
        {
            var RoutineTaskDAL = new RoutineTaskDAL(_iconfiguration);
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<RoutineModel, Routine>();
            });

            IMapper mapper = config.CreateMapper();
            var source = objRoutine; //new RoutineModel();
            var dest = mapper.Map<RoutineModel, Routine>(source);
            bool response = RoutineTaskDAL.InsertRoutineTask(dest);
            return response;
        }
        public async Task<IActionResult> AddTask(Models.RoutineModel objR)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            string role = HttpContext.Session.GetString(SessionRole);
            string ecno = HttpContext.Session.GetString(SessionECNo);
            string assignTo = HttpContext.Session.GetString(SessionAssignTo);
            ViewBag.ECNo = ecno;
            Models.RoutineModel objTemp = new Models.RoutineModel();
            string execDate = string.Empty;
            DateTime dExeDate = objR.ExecutionDate;
            DateTime dNxtExeDate;
            if (objR.ID == 0)
            {
                execDate = dExeDate.ToString("yyyy-MM-dd ");
                dExeDate = Convert.ToDateTime(execDate + new TimeSpan(07, 00, 00));
                dNxtExeDate = dExeDate;
            }
            else
            {
                execDate = dExeDate.ToString("yyyy-MM-dd HH:mm:ss");
                dNxtExeDate = dExeDate;
            }
            try
            {
                if (objR.ID == 0 || objR.TaskId=="")
                {
                    objR.TaskId = objR.ShortDescription.Substring(0, 1).ToUpper() + objR.ShortDescription.Substring(objR.ShortDescription.Length - 1, 1).ToUpper() + DateTime.Now.ToString("HHmmss");
                        objR.CreatorId = ecno;
                    objR.CreatorName = HttpContext.Session.GetString(SessionName);
                    objR.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    switch(objR.Frequency)
                    {
                        case "Daily":
                            while (dNxtExeDate<=dExeDate.AddMonths(1))
                            {
                                objR.ExecutionDate = dNxtExeDate;
                                bool response= InsertRoutineTask(objR);
                                dNxtExeDate = dNxtExeDate.AddDays(1);
                            }
                            break;
                        case "Weekly":
                            while (dNxtExeDate <= dExeDate.AddMonths(1))
                            {
                                objR.ExecutionDate = dNxtExeDate;
                                bool response = InsertRoutineTask(objR);
                                dNxtExeDate = dNxtExeDate.AddDays(1);
                            }
                            break;
                    }
                    
                }
                else
                {
                    if (objR.CompletedFlg == "N")
                    {
                        objR.ClosureDate = string.Empty;
                    }
                    else
                    {
                        objR.ClosureDate = DateTime.Now.ToString("yyyy-MM-dd");
                        if(string.IsNullOrEmpty(objR.Remarks))
                        {
                            objR.Remarks = "Completed by "+objR.CreatorName;
                        }
                    }

                    _Db.tbl_Routine.Update(objR);
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("RoutineList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("RoutineList");
            }
        }
        public async Task<IActionResult> DeleteTask(int id)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            try
            {
                var inci = await _Db.tbl_Routine.FindAsync(id);
                if (inci != null)
                {
                    _Db.tbl_Routine.Remove(inci);
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("RoutineList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("RoutineList");
            }
        }
        public IActionResult RoutineHistory(string ID)
        {
            try
            {
                CompositeSOCModel comp = new CompositeSOCModel();
                GetAppSettingsFile();
                List<Routine> objTaskList = GetRoutineHistoryDetails(ID);
                comp.RoutineComp = objTaskList;
                return View(comp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
