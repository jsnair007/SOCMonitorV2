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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace SOCMonitorV2.Controllers
{
    public class HomeController : Controller
    {
       
        const string SessionECNo = "_ECNo";
        const string SessionRole = "_Role";
        const string SessionName = "_Name";
        const string SessionDesignation = "_Designation"; 

        private readonly ILogger<HomeController> _logger;
        private static IConfiguration _iconfiguration;
        string SessionId = string.Empty;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Home()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.ECNo = HttpContext.Session.GetString(SessionECNo);
            ViewBag.Designation= HttpContext.Session.GetString(SessionDesignation);
            CompositeSOCModel comp = new CompositeSOCModel();
            //get admin pending task for superadmin login
            if (ViewBag.Role == "SuperAdmin" || ViewBag.Role == "Admin")
            {
                GetAppSettingsFile();
                List<Employee> objEmp = GetConsolidatedPendingTaskCount(ViewBag.Role, ViewBag.ECNo);
                comp.Employees = objEmp;
            }
            //get routine details
            GetAppSettingsFile();
            List<Routine> objRoutine = GetRoutineTaskDetails(DateTime.Now.ToString("yyyy-MM-dd 07:00:00"), DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
            comp.RoutineComp = objRoutine;
            
            GetAppSettingsFile();
            List<TaskClass> objTaskList = GetTaskDetails(ViewBag.Role, ViewBag.ECNo,ViewBag.Designation);

            if (objTaskList.Count != 0)
            {
                if (objTaskList[0].ResponseCode == "999")
                {
                    ViewBag.ErrorId = "999";
                    return RedirectToAction("Error", ViewBag.ErrorId);
                }
            }
            comp.Tasks = objTaskList;

            //get shift staff details
            GetAppSettingsFile();
            List<ShiftSchedule> objShiftList = GetShiftScheduleDetails();
            comp.ShiftSchedules = objShiftList;

            //get staff details including vendor
            return View(comp);
        }
        public IActionResult Index(LoginModel objLogin)
        {
            try
            {
                CompositeSOCModel compModel = new CompositeSOCModel();
                //clsEvent.LogEvent("Inside Index page", "Reached inside of Index of Login page. Login id: " + objLogin.ECNo + ", Password: " + objLogin.Password);
                //validate Login credentials
                string Flg = "N";
                string currDate = DateTime.Now.ToString("yyyy-MM-dd");
                GetAppSettingsFile();
                List<ClassLibraryDB.Model.LoginClass> objLoginList = GetLoginDetails(objLogin);
                clsEvent.LogEvent("Login details: ","ECNo:"+ objLoginList[0].ECNo +" ,Name: "+objLoginList[0].Name +" ,Role: "+objLoginList[0].Role );
                compModel.ECNo = objLogin.ECNo;
                if (objLoginList[0].Active == "N")
                {
                    ViewBag.Name = objLoginList[0].Name;
                    ViewBag.Role = objLoginList[0].Role;
                    compModel.Logins = objLoginList[0];
                    return View("ChangePassword", compModel);
                }
                if (objLoginList[0].ResponseCode == "000")
                {
                    HttpContext.Session.SetString(SessionDesignation, objLoginList[0].Designation);
                    HttpContext.Session.SetString(SessionName, objLoginList[0].Name);
                    HttpContext.Session.SetString(SessionECNo, objLogin.ECNo);
                    HttpContext.Session.SetString(SessionRole, objLoginList[0].Role);

                    if (objLoginList[0].Role != "Admin" && objLoginList[0].Role != "SuperAdmin")
                    {
                        //Mark attendance
                        string Name = objLoginList[0].Name;
                        string ecno = objLogin.ECNo;
                        GetAppSettingsFile();

                        List<Attendance> objAL = GetAttendanceDetails(ecno);
                        ViewBag.Name = Name;
                        compModel = new CompositeSOCModel();
                        compModel.AttendanceModels = objAL;
                        AttendanceModel att = new AttendanceModel();
                        if (objAL.Count != 0)
                        {
                            for (int i = 0; i < objAL.Count; i++)
                            {
                                if (objAL[i].IsPresent.Trim() != "Y")
                                {
                                    ViewBag.ECNo = objLogin.ECNo;
                                    if (currDate == objAL[i].DutyDate)
                                    {
                                        Flg = "Y";
                                        ViewBag.ShiftDateMatch = "Y";
                                        ViewBag.IsPresent = objAL[i].IsPresent.Trim();
                                        ViewBag.ShiftTimeBase = objAL[i].ShiftTimeBase;
                                        ViewBag.DutyStartTime = objAL[i].DutyStartTime;
                                        ViewBag.DutyEndTime = objAL[i].DutyEndTime;
                                        ViewBag.DutyDate = objAL[i].DutyDate;
                                        DateTime dStartDt = Convert.ToDateTime(objAL[i].DutyStartTime);
                                        DateTime dThresholdTime = dStartDt.AddHours(2);

                                        if (DateTime.Now > dThresholdTime)
                                        {
                                            ViewBag.ShiftDateMatch = "T"; //crossed threshold time of 2 hourse to mark attendance
                                            return View("MarkAttendanceIndex", compModel);
                                        }
                                        if (objAL[i].IsPresent == "Y")
                                        {
                                            //objLoginList[0].SessionId = Guid.NewGuid().ToString();
                                            SessionId = Guid.NewGuid().ToString();
                                            HttpContext.Session.SetString(SessionId, Guid.NewGuid().ToString());
                                            objLoginList[0].SessionId = SessionId;
                                            ViewBag.Role = objLoginList[0].Role;
                                            CompositeModel comp = new CompositeModel();
                                            //get pending task
                                            GetAppSettingsFile();
                                            //if super admin logins, list out all task, else corresponding employee task only

                                            List<TaskClass> objTaskList = GetTaskDetails(objLoginList[0].Role, objLogin.ECNo, objLoginList[0].Designation);
                                            comp.Tasks = objTaskList;

                                            //get shift staff details
                                            GetAppSettingsFile();
                                            List<ShiftSchedule> objShiftList = GetShiftScheduleDetails();
                                            comp.ShiftSchedules = objShiftList;

                                            //get staff details including vendor
                                            return View(comp);
                                        }
                                        else
                                        {
                                            ViewBag.ShiftDateMatch = "Y";
                                            return View("MarkAttendanceIndex", compModel);
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.DutyDate = objAL[i].DutyDate;
                                        ViewBag.ShiftDateMatch = "F"; //found in 5 days of time
                                        return View("MarkAttendanceIndex", compModel);
                                    }
                                }
                                else
                                {
                                    CompositeSOCModel comp = new CompositeSOCModel();
                                    //get pending task
                                    GetAppSettingsFile();
                                    //if super admin logins, list out all task, else corresponding employee task only
                                    List<TaskClass> objTaskList = GetTaskDetails(objLoginList[0].Role, objLogin.ECNo, objLoginList[0].ReportingAuth);
                                    comp.Tasks = objTaskList;

                                    //get shift staff details
                                    GetAppSettingsFile();
                                    List<ShiftSchedule> objShiftList = GetShiftScheduleDetails();
                                    comp.ShiftSchedules = objShiftList;
                                    ViewBag.Name = objAL[i].Name;
                                    ViewBag.IsPresent = objAL[i].IsPresent;
                                    return View(comp); //attendance already marked. Goto index.cshtml page
                                                       //return RedirectToAction("Home",comp);
                                }
                            }
                        }
                        else
                        {
                            ViewBag.ShiftDateMatch = "N";
                            return View("MarkAttendanceIndex", compModel);
                        }
                    }
                    else
                    {
                        CompositeSOCModel comp = new CompositeSOCModel();
                        //get pending task
                        GetAppSettingsFile();
                        //if super admin logins, list out all task, else corresponding employee task only

                        List<TaskClass> objTaskList = GetTaskDetails(objLoginList[0].Role, objLogin.ECNo, objLoginList[0].Designation);
                        comp.Tasks = objTaskList;

                        //get shift staff details
                        GetAppSettingsFile();
                        List<ShiftSchedule> objShiftList = GetShiftScheduleDetails();
                        comp.ShiftSchedules = objShiftList;

                        ViewBag.Name = objLoginList[0].Name;
                        ViewBag.Role = objLoginList[0].Role;
                        return View(comp);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login!");
                    return View("Login", objLogin);
                }
            }catch(Exception ee)
            {
                clsEvent.LogEvent("Exception in HomeController.Index page. Message is: ", ee.Message);
            }
            return View();
        }

        static List<ClassLibraryDB.Model.LoginClass> GetLoginDetails(LoginModel objLogin)
        {
            var loginDAL = new LoginDAL(_iconfiguration);
            List<ClassLibraryDB.Model.LoginClass> objLoginList = new List<ClassLibraryDB.Model.LoginClass>();
            try
            {
                string hashPwd = loginDAL.GetHashPwd(objLogin.ECNo);
                if (BCrypt.Net.BCrypt.Verify(objLogin.Password, hashPwd))
                {
                    objLogin.Password = hashPwd;
                    //clsEvent.LogEvent("Inside GetLoginDetails function", "hashPwd: "+hashPwd);
                    var listLoginModel = loginDAL.GetList(objLogin.ECNo, objLogin.Password);
                    objLoginList = listLoginModel;
                }
                else
                {
                    LoginClass objClass = new LoginClass();
                    objClass.ResponseCode = "999";
                    objLoginList.Add(objClass);
                }
            }
            catch (Exception Ex)
            {
                LoginClass objClass = new LoginClass();
                objClass.ResponseCode = "999";
                objLoginList.Add(objClass);
                clsEvent.LogEvent("Exception in GetLoginDetails(). Error is: ", Ex.Message);
            }
            return objLoginList;
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
            catch(Exception ee)
            {
                clsEvent.LogEvent("SQL Connection exception: ", ee.Message);
            }
        }
        [HttpPost]
        public ActionResult MarkAttendanceIndex()
        {
            //Mark attendance
            string ECNo = HttpContext.Request.Form["ECNo"];
            string Name = HttpContext.Request.Form["Name"];
            string DutyDate = HttpContext.Request.Form["DutyDate"];
            string ShiftBase = HttpContext.Request.Form["ShiftBase"];
            GetAppSettingsFile();
            var AttendanceDAL = new AttendanceDAL(_iconfiguration);
            bool response = AttendanceDAL.MarkAttendance(ECNo, Name, DutyDate,ShiftBase);
            if (response)
            {
                ViewBag.Name = Name;
                return RedirectToAction("Home");
            }
            else
                ViewBag.Message = "Unable to mark attendance!";
            return View();
        }
        
        static List<Attendance> GetAttendanceDetails(string ecno)
        {
            var AttendanceDAL = new AttendanceDAL(_iconfiguration);
            List<Attendance> attendanceList = AttendanceDAL.GetShiftScheduleDetailsForAttendance(ecno);
            List<Attendance> objAL = attendanceList;
            return objAL;
        }

        static List<Employee> GetConsolidatedPendingTaskCount(string role,string ECNo)
        {
            var LoginDAL = new LoginDAL(_iconfiguration);
            var listEmp = LoginDAL.GetPendingTaskCount(role,ECNo);
            List<Employee> objEmp= new List<Employee>();
            objEmp = listEmp;
            return objEmp;
        }
        static List<ClassLibraryDB.Model.Routine> GetRoutineTaskDetails(string StartDate, string EndDate)
        {
            var RoutineTaskDAL = new RoutineTaskDAL(_iconfiguration);
            var listTask = RoutineTaskDAL.GetRoutineList(StartDate, EndDate);
            List<ClassLibraryDB.Model.Routine> objTaskList = new List<ClassLibraryDB.Model.Routine>();
            objTaskList = listTask;
            return objTaskList;
        }
        static List<TaskClass> GetTaskDetails(string Role, string ECNo,string Designation)
        {
            var TaskDAL = new TaskDAL(_iconfiguration);
            var listTask = TaskDAL.GetList(Role, ECNo, Designation);
            List<TaskClass> objTaskList = new List<TaskClass>();
            objTaskList = listTask;
            return objTaskList;
        }
        static List<ShiftSchedule> GetShiftScheduleDetails()
        {
            var ShiftScheduleDAL = new ShiftScheduleDAL(_iconfiguration);
            var listShift = ShiftScheduleDAL.GetShiftScheduleList();
            List<ShiftSchedule> objShiftList = new List<ShiftSchedule>();
            objShiftList = listShift;
            return objShiftList;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Logout()
        {
            SessionId = string.Empty;
            HttpContext.Session.Clear();
            return View("Login");
        }
        //[Route("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    await _accountRespository.SignOutAsync();
        //    return RedirectToAction("Home", "Home");
        //}

        [HttpPost]
        public ActionResult ChangePassword(CompositeSOCModel model)
        {
            //ViewBag.Name =model.Logins.Name;
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            if (ModelState.IsValid)
            {
                if (model.ChangePasswordModel.CurrentPassword == model.ChangePasswordModel.NewPassword)
                {
                    ViewBag.Response = "Old Password and new password cannot be same! ";
                    return View();
                }
                else
                {
                    PasswordScore score= PasswordAdvisor.CheckStrength(model.ChangePasswordModel.ConfirmNewPassword);
                    if (score==PasswordScore.Strong || score==PasswordScore.VeryStrong)
                    {
                        string NewPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.ChangePasswordModel.ConfirmNewPassword);
                        GetAppSettingsFile();
                        var AttendanceDAL = new AttendanceDAL(_iconfiguration);
                        bool response = AttendanceDAL.ChangePasswordFun(model.ECNo, NewPasswordHash,"ChangePassword");
                        if (response)
                        {
                            ViewBag.Name = model.Logins.Name;
                            ViewBag.Role = model.Logins.Role;

                            HttpContext.Session.SetString(SessionDesignation, model.Logins.Designation);
                            HttpContext.Session.SetString(SessionName, model.Logins.Name);
                            HttpContext.Session.SetString(SessionECNo, model.ECNo);
                            HttpContext.Session.SetString(SessionRole, model.Logins.Role);

                            return View("Index");
                        }
                        else
                        {
                            ViewBag.Message = "Unable to update new password!";
                        }
                    }
                    else
                    {
                        ViewBag.Response ="Password strength is ["+ score+"]. Try again";
                        return View();
                    }
                }
            }
            return View();
        }
    }
}
