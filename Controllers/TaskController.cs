using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryDB.DAL;
using ClassLibraryDB.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SOCMonitorV2.Models;

namespace SOCMonitorV2.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskContext _Db;
        const string SessionName = "_Name";
        const string SessionDesignation = "_Designation";
        const string SessionRole = "_Role";
        const string SessionECNo = "_ECNo";
        const string SessionAssignTo = "_AssignTo";
        const string SessionErrorMessage = "_ErrorMessage";
        public TaskController(TaskContext Db)
        {
            _Db = Db;
        }

        // GET: Task
        public IActionResult TaskList()
        {
            string role = HttpContext.Session.GetString(SessionRole);
            string desig = HttpContext.Session.GetString(SessionDesignation);
            string ecno = HttpContext.Session.GetString(SessionECNo);
            ViewBag.Role = role;
            ViewBag.Name= HttpContext.Session.GetString(SessionName);
            ViewBag.ECNo = ecno;
            string sql = string.Empty;
            try
            {
                //var taskList = _Db.tbl_Task.ToList();
                switch (role)
                {
                    case "User":
                    case "SuperUser":
                        sql = " select t.ID,ShortDescription,FullDescription,CreatedDate,CreatorId,e.Name CreatorName,ee.Name AssignedBy, eee.Name AssignTo,AssignDate," +
                              " ETC,TaskStatus,Priority,ClosureDate,Remarks,ExecutivesComments from tbl_Task t left join tbl_Employee e "+
                              " on t.CreatorId=e.ECNo join tbl_Employee ee on t.AssignedBy=ee.ECNo join tbl_Employee eee on t.AssignTo=eee.ECNo "+
                              " where t.TaskStatus in('Open','PendingForClosure') and  " +
                              " (t.AssignTo=@ECNo or t.CreatorId=@ECNo or t.AssignedBy=@ECNo) order by AssignDate,ETC ";
                        break;
                    case "Admin":
                        sql = " select t.ID,ShortDescription,FullDescription,CreatedDate,CreatorId,e.Name CreatorName,ee.Name AssignedBy,eee.Name AssignTo,AssignDate," +
                              " ETC,TaskStatus,Priority,ClosureDate,Remarks,ExecutivesComments from tbl_Task t left join tbl_Employee e "+
                              " on t.CreatorId=e.ECNo join tbl_Employee ee on t.AssignedBy=ee.ECNo join tbl_Employee eee on t.AssignTo=eee.ECNo " +
                              " where (t.AssignTo in( " +
                              " select ECNo from tbl_Employee where ReportingAuth = @ECNo) or (t.AssignTo=@ECNo or t.CreatorId=@ECNo or t.AssignedBy=@ECNo) ) order by AssignedBy, AssignDate,ETC";
                        break;
                    case "SuperAdmin":
                        sql = " select t.ID,ShortDescription,FullDescription,CreatedDate,CreatorId,e.Name CreatorName,ee.Name AssignedBy,eee.Name AssignTo,AssignDate," +
                              " ETC,TaskStatus,Priority,ClosureDate,Remarks,ExecutivesComments from tbl_Task t left join tbl_Employee e "+
                              " on t.CreatorId=e.ECNo join tbl_Employee ee on t.AssignedBy=ee.ECNo join tbl_Employee eee on t.AssignTo=eee.ECNo " +
                              "  order by AssignedBy, AssignDate,ETC";
                        break;
                }
                //var taskList = _Db.tbl_Task.ToList().Where(c => c.AssignTo == HttpContext.Session.GetString(SessionECNo));//working fine
                //var taskList = _Db.tbl_Task.FromSqlRaw(sql).ToList();

                var param1 = new SqlParameter("Designation", desig);
                var param2 = new SqlParameter("ECNo", ecno);
                var taskList = _Db.tbl_Task.FromSqlRaw(sql, param1,param2).ToList();
                return View(taskList);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
       
        // GET: Task/Create
        public IActionResult Create(TaskModel objT)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            if (objT.AssignTo != null) //to fetch default AssignedTo value in dropdown box
            {
                ViewBag.AssignTo = objT.AssignTo;
                string sql = "select ECNo,Name from tbl_Officials o left join tbl_Task t on o.ECNo=t.AssignTo where t.ID=@id ";
                var param = new SqlParameter("id", objT.ID);
                var list = _Db.tbl_Officials.FromSqlRaw(sql, param).ToList();
                ViewBag.AssignToDetails = new List<SelectListItem>
                {
                    new SelectListItem {Text = list[0].Name, Value = list[0].ECNo},
                };
                HttpContext.Session.SetString(SessionAssignTo, list[0].ECNo);
                loadDDL(list[0].ECNo,list[0].Name);
            }
            else
                loadDDL("", "");
            return View(objT);
        }
        public async Task<IActionResult> AddTask(TaskModel objT)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            string role = HttpContext.Session.GetString(SessionRole);
            string ecno= HttpContext.Session.GetString(SessionECNo);
            string assignTo = HttpContext.Session.GetString(SessionAssignTo);
            ViewBag.ECNo = ecno;
            TaskModel objTemp = new TaskModel();
            try
            {
                //sql = "select * from tbl_Employee where ECNo=@SelectedECNo ";
                //var param = new SqlParameter("SelectedECNo", objT.AssignTo);
                //var tmp = _Db.tbl_Employee.FromSqlRaw(sql, param).ToList();
                //objT.AssignTo = tmp[0].Name;


                if (objT.ID == 0)
                {
                    objT.CreatorId = ecno;
                    objT.CreatorName = HttpContext.Session.GetString(SessionName);
                    objT.AssignedBy = ecno;
                    objT.AssignDate = DateTime.Now.ToString("yyyy-MM-dd");
                    objT.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    _Db.tbl_Task.Add(objT);
                    await _Db.SaveChangesAsync();
                }
                else
                {
                    objT.AssignDate = DateTime.Now.ToString("yyyy-MM-dd");
                    if(objT.TaskStatus=="Closed")
                    {
                        if(role=="User" || role=="SuperUser" )
                        {
                            objT.TaskStatus = "PendingForClosure";
                            objT.ClosureDate = DateTime.Now.ToString("yyyy-MM-dd");
                            objT.Remarks = objT.Remarks + "\n[Request for closure by: " + HttpContext.Session.GetString(SessionName) + "]";
                            objT.AssignedBy = objT.CreatorId;
                        }
                        else //any executives can close the task
                        {
                            objT.TaskStatus = "Closed";
                            objT.AssignedBy = objT.CreatorId;
                            objT.Remarks = objT.Remarks + "\n[Closed by: " + HttpContext.Session.GetString(SessionName) + "]";
                        }
                    }
                  
                    if (objT.TaskStatus == "Open")
                    {
                        objT.ClosureDate = string.Empty;
                        objT.AssignedBy = ecno;
                    }
                    if(objT.TaskStatus== "Re-Open")
                    {
                        objT.Remarks = "[Task Re-Opened by: "+ HttpContext.Session.GetString(SessionName) + "]!";
                        objT.ClosureDate = string.Empty;
                        objT.AssignedBy = ecno;
                        objT.TaskStatus = "Open";
                    }
                 
                    _Db.Entry(objT).State = EntityState.Modified;
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("TaskList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("TaskList");
            }
        }

        public IActionResult FullDesciption(TaskModel objFD)
        {
            return View(objFD);
        }

        public IActionResult TaskHistory(TaskHistoryModel objTH)
        {
            try
            {
                //string sql = "select * from tbl_Task_History where Task_ID=@id order by R_Mod_Time";
                string sql = " select th.ID,Task_ID,ShortDescription,FullDescription,CreatedDate,CreatorId,e.Name CreatorName,ee.Name AssignedBy,eee.Name AssignTo,AssignDate, " +
                            " ETC,TaskStatus,Priority,ClosureDate,Remarks,ExecutivesComments,R_Mod_Time,eee.Name ModifiedBy " +
                            " from tbl_Task_History th left join tbl_Employee e on th.CreatorId = e.ECNo " +
                            " join tbl_Employee ee on th.AssignedBy = ee.ECNo join tbl_Employee eee on th.AssignTo = eee.ECNo " +
                            " where Task_ID = @id order by R_Mod_Time ";
                var param = new SqlParameter("id", objTH.ID);
                var taskHistory = _Db.tbl_Task_History.FromSqlRaw(sql, param).ToList();
                return View(taskHistory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteTask(int id)
        {
            ViewBag.Role = HttpContext.Session.GetString(SessionRole);
            ViewBag.Name = HttpContext.Session.GetString(SessionName);
            try
            {
                var inci = await _Db.tbl_Task.FindAsync(id);
                if (inci != null)
                {
                    _Db.tbl_Task.Remove(inci);
                    await _Db.SaveChangesAsync();
                }
                return RedirectToAction("TaskList");
            }
            catch (Exception ex)
            {
                return RedirectToAction("TaskList");
            }
        }


        private void loadDDL(string id,string value)
        {
            try
            {
                List<OfficialsModel> offList = new List<OfficialsModel>();
                offList = _Db.tbl_Officials.ToList();
                if (id == string.Empty)
                    offList.Insert(0, new OfficialsModel { ECNo = "", Name = "-- Task Assign To? --" });
                else
                    offList.Insert(0, new OfficialsModel { ECNo = id, Name = value });
                ViewBag.DepList = offList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
