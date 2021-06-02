using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryDB.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SOCMonitorV2.Models;
using Microsoft.Data.SqlClient;
using System.Drawing;

namespace SOCMonitorV2.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeContext _context;
        const string SessionName = "_Name";
        const string SessionRole = "_Role";
        const string SessionECNo = "_ECNo"; 
        const string SessionDesignation = "_Designation";
        const string SessionResponse = "_Response";
        const string SessionReportingAuth = "_ReportingAuth"; 
        private static IConfiguration _iconfiguration;
        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }
        public IActionResult EmployeeList()
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            var empList = _context.tbl_Employee.ToList();
            return View(empList);
        }
        public IActionResult Create(EmployeeModel empModel)
        {
            try
            {
                ViewBag.Role = HttpContext.Session.GetString(SessionRole);
                ViewBag.Name = HttpContext.Session.GetString(SessionName);
                if (empModel.ReportingAuth != null) //to fetch default ReportingAuth value from dropdown box
                {
                    ViewBag.ReportingAuth = empModel.ReportingAuth;
                    string sql = "select exe.ECNo,exe.Name from tbl_Executives exe left join tbl_Employee emp on exe.ECNo=emp.ReportingAuth where emp.ID=@id ";
                    var param = new SqlParameter("id", empModel.ID);
                    var list = _context.tbl_Executives.FromSqlRaw(sql, param).ToList();
                    ViewBag.SessionReportingAuth = new List<SelectListItem>
                {
                    new SelectListItem {Text = list[0].Name, Value = list[0].ECNo},
                };
                    HttpContext.Session.SetString(SessionReportingAuth, list[0].ECNo);
                    loadDDL(list[0].ECNo, list[0].Name);
                }
                else
                    loadDDL("", "");
            }catch(Exception ee)
            {
                clsEvent.LogEvent("Exception in EmployeeController/Create", ee.Message);
            }
            return View(empModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(EmployeeModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                if (employeeModel.ID == 0)
                {
                    if (employeeModel.IsExecutive == true)
                    {
                        employeeModel.ReportingAuth = employeeModel.ECNo;
                        employeeModel.Role = "Admin";
                    }
                    _context.tbl_Employee.Add(employeeModel);
                    await _context.SaveChangesAsync();
                    GetAppSettingsFile();
                    LoginModel objLogin = new LoginModel();
                    objLogin.ECNo = employeeModel.ECNo;
                    string defaultPwd = "SOC@123";
                    objLogin.Password = BCrypt.Net.BCrypt.HashPassword(defaultPwd);   
                    bool createLogin = CreateLoginCredential(objLogin);
                }
                else
                {
                    _context.Entry(employeeModel).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("EmployeeList");
        }
        public async Task<IActionResult> DeleteEmployee(int id,string ECNo)
        {
            try
            {
                var sh = await _context.tbl_Employee.FindAsync(id);
                if (sh != null)
                {
                    _context.tbl_Employee.Remove(sh);
                    await _context.SaveChangesAsync();
                    GetAppSettingsFile();
                    LoginModel objLogin = new LoginModel();
                    objLogin.ECNo = ECNo;
                    bool deleteLogin = DeleteLogin(objLogin);
                }
                return RedirectToAction("EmployeeList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("EmployeeList");
            }
        }

        static bool CreateLoginCredential(LoginModel objLogin)
        {
            var loginDAL = new LoginDAL(_iconfiguration);
            bool response = loginDAL.CreateLoginCredential(objLogin.ECNo, objLogin.Password);
            return response;
        }
        static bool DeleteLogin(LoginModel objLogin)
        {
            var loginDAL = new LoginDAL(_iconfiguration);
            bool response = loginDAL.DeleteLogin(objLogin.ECNo);
            return response;
        }
        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        public IActionResult ResetPassword(ResetPwdModel rstModel)
        {
            try
            {
                loadOfficials("", "");
                ViewBag.Role = HttpContext.Session.GetString(SessionRole);
                ViewBag.Name = HttpContext.Session.GetString(SessionName);

            }
            catch (Exception ee)
            {
                clsEvent.LogEvent("Exception in ResetPassword.", ee.Message);
            }
            return View();
        }
        [HttpPost]
        public IActionResult ResetPwd(ResetPwdModel rstModel)
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            try
            {
                string defaultPwd = "SOC@123";
                string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPwd);
                GetAppSettingsFile();
                var AttendanceDAL = new AttendanceDAL(_iconfiguration);
                bool response = AttendanceDAL.ChangePasswordFun(rstModel.ECNo, defaultPasswordHash, "ResetPwd");
                if (response)
                {
                    var LoginDAL = new LoginDAL(_iconfiguration);
                    List<ClassLibraryDB.Model.LoginClass> objLoginList = new List<ClassLibraryDB.Model.LoginClass>();
                    var listLoginModel = LoginDAL.GetList(rstModel.ECNo, defaultPasswordHash);
                    objLoginList = listLoginModel;

                    //ViewBag.Name = objLoginList[0].Name;
                    //ViewBag.Role = objLoginList[0].Role;

                    //HttpContext.Session.SetString(SessionDesignation, objLoginList[0].Designation);
                    //HttpContext.Session.SetString(SessionName, objLoginList[0].Name);
                    //HttpContext.Session.SetString(SessionECNo, rstModel.ECNo);
                    //HttpContext.Session.SetString(SessionRole, objLoginList[0].Role);

                    rstModel.Response = objLoginList[0].Name+" ["+ rstModel.ECNo+ "] Password successfully reset to Default ";
                }
                else
                {
                    rstModel.Response = "Failed to Reset Password! ";
                }
            }catch(Exception ee)
            {
                rstModel.Response = "Exception while resetting password!";
                clsEvent.LogEvent("ChangePassword: Reset Password exception.", ee.Message);
            }
            return View(rstModel);
        }
        private void loadDDL(string id, string value)
        {
            try
            {
                List<ExecutiveModel> exeList = new List<ExecutiveModel>();
                exeList = _context.tbl_Executives.ToList();
                if (id == string.Empty)
                    exeList.Insert(0, new ExecutiveModel { ECNo = "", Name = "-- Reporting To? --" });
                else
                    exeList.Insert(0, new ExecutiveModel { ECNo = id, Name = value });
                ViewBag.AuthList = exeList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void loadOfficials(string id, string value)
        {
            try
            {
                List<OfficialsModel> offList = new List<OfficialsModel>();
                offList = _context.tbl_Officials.ToList();
                if (id == string.Empty)
                    offList.Insert(0, new OfficialsModel { ECNo = "", Name = "-- Select Any --" });
                else
                    offList.Insert(0, new OfficialsModel { ECNo = id, Name = value });
                ViewBag.OffList = offList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
