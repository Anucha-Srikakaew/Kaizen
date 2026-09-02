using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ActivitySystem.ViewModel;
using ActivitySystem.Data;
using ActivitySystem.Models;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Http;

namespace ActivitySystem.Controllers
{
    public class LoginController : Controller
    {
        //ILogger<LoginController> _logger;  ILogger<LoginController> logger
        public DatabaseContext Context { get; }
        public LoginController(DatabaseContext context)
        {
            this.Context = context;
            //_logger = logger;
        }

        const string SessionEmpNumRow   = "_EmpNumRow";
        const string SessionEmpId       = "_EmpId";
        const string SessionEmpName     = "_EmpName";
        const string SessionEmpBirth    = "_EmpBirth";
        const string SessionEmpAdmin    = "_EmpAdmin";
        const string SessionEmpLeader   = "_EmpLeader";
        const string SessionEmpManager  = "_EmpManager";

        //public async Task<IActionResult> Index()
        //{
        //    Session();
        //    return View();
        //}
        //Test public ActionResult Session(String EmpId)

        public ActionResult Session()
        {
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId != null)
            {
                ViewBag.NumRow  = HttpContext.Session.GetString(SessionEmpNumRow);
                ViewBag.Id      = HttpContext.Session.GetString(SessionEmpId);
                ViewBag.Name    = HttpContext.Session.GetString(SessionEmpName);
                ViewBag.Birth   = HttpContext.Session.GetString(SessionEmpBirth);
                ViewBag.Admin   = HttpContext.Session.GetString(SessionEmpAdmin);
                ViewBag.Leader  = HttpContext.Session.GetString(SessionEmpLeader);
                ViewBag.Manager = HttpContext.Session.GetString(SessionEmpManager);
                return RedirectToAction("UserProfile", "Login");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        public ActionResult Login()
        {
            //Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId != null)
            {
                return RedirectToAction("Dashboard", "Login");
            }
            else
            {
                return View("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(View_ApprovalFlow objData)
        {
            //if (ModelState.IsValid)
           try
            {
  
                //string date_str = objData.EMP_BIRTH.ToString("dd/MM/yyyy");
                //string id = objData.EMP_ID.ToString();
                //string birth = objData.EMP_BIRTH.ToString();

                var Log = Context.View_ApprovalFlow.Where(a => a.PRS_NO.Equals(objData.PRS_NO) && a.EMP_BIRTH.Equals(objData.EMP_BIRTH)).DefaultIfEmpty().FirstOrDefault();
                {
                    if (Log != null)
                    {
                        string numrow   = Log.Num_Row.ToString();
                        string id       = Log.PRS_NO.ToString();
                        string name     = Log.EMP_E_NAME.ToString();
                        string birth    = Log.EMP_BIRTH.ToString();
                        string admin    = "";
                        string leader   = "";
                        string manager  = "";

                        //Check Permission for Admin
                        var chkAdmin = Context.Administrator.Where(a => a.EmpId.Equals(Log.PRS_NO)).DefaultIfEmpty().FirstOrDefault();
                        if (chkAdmin != null)
                        {
                            admin = "Admin";
                        }

                        //Check Permission for Leader
                        var chkLeadder = Context.View_Leader.Where(a => a.Approval_No.Equals(Log.PRS_NO)).DefaultIfEmpty().FirstOrDefault();
                        if (chkLeadder != null)
                        {
                            leader = "Leader";
                        }
                        else if (id == "96624")
                        {
                            leader = "Leader"; //โคคิชิ โมริกามิ she only
                        }


                            //Check Permission for Manager 
                            var chkManager = Context.View_Manager.Where(a => a.Approval_No.Equals(Log.PRS_NO)).DefaultIfEmpty().FirstOrDefault();
                        if (chkManager != null)
                        {
                            manager = "Manager";
                        }
                        else if (id == "96624")
                        {
                            manager = "Manager"; //โคคิชิ โมริกามิ she only
                        }

                        HttpContext.Session.SetString(SessionEmpNumRow, numrow);
                        HttpContext.Session.SetString(SessionEmpId, id);
                        HttpContext.Session.SetString(SessionEmpName, name);
                        HttpContext.Session.SetString(SessionEmpBirth, birth);
                        HttpContext.Session.SetString(SessionEmpAdmin, admin);
                        HttpContext.Session.SetString(SessionEmpLeader, leader);
                        HttpContext.Session.SetString(SessionEmpManager, manager);

                        //TempData["numrow"] = HttpContext.Session.GetString(SessionEmpNumRow);
                        //TempData["id"] = HttpContext.Session.GetString(SessionEmpId);
                        //TempData["name"] = HttpContext.Session.GetString(SessionEmpName);
                        //TempData["birth"] = HttpContext.Session.GetString(SessionEmpBirth);
                        //TempData["admin"] = HttpContext.Session.GetString(SessionEmpAdmin);
                        //TempData["leader"] = HttpContext.Session.GetString(SessionEmpLeader);
                        //TempData["manager"] = HttpContext.Session.GetString(SessionEmpManager);

                        //return RedirectToAction("Profile", "Login");
                        Dashboard();
                        return View("Dashboard");
                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "Employee ID or Your Birthday (CE) is wrong";
                        ModelState.AddModelError("", "ID/Birth is incorrect.");
                    }
                    //return View("Index");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(objData.PRS_NO))
                {
                    TempData["notice"] = "Can't connect server!!";
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

             return View("Index");
        }

        public IActionResult Dashboard()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)     
            {
                return RedirectToAction("Login", "Login");
            }

            DateTime DateTimeNow = DateTime.Now;

            DateTime StartingDate = DateTimeNow;
            DateTime EndingDate   = DateTimeNow;

            var getDueDate = Context.DueDate.ToList();
            foreach (var itemDueDate in getDueDate)
            {
                var Start   = itemDueDate.StartingDate;
                var End     = itemDueDate.EndingDate;

                if (DateTimeNow >= Start && DateTimeNow <= End)
                {
                    StartingDate = itemDueDate.StartingDate;
                    EndingDate   = itemDueDate.EndingDate;
                }

            }

            //var getDueDate = Context.DueDate.OrderByDescending(i => i.DueDateNo).FirstOrDefault();
            //DateTime StartingDate = getDueDate.StartingDate;
            //DateTime EndingDate = getDueDate.EndingDate;

            var view_kaizen = Context.View_Kaizens.Where(m => m.Date >= StartingDate && m.Date <= EndingDate && (m.KzStatus.Equals("Completed") || m.KzStatus.Equals("Award"))).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

            //Show Update Now
            ViewBag.ShowDateNow = StartingDate.ToString("dd/MM/yyyy") + " - " + EndingDate.ToString("dd/MM/yyyy");

            //Cnt
            var cntKaizen = view_kaizen.Count();
            ViewBag.ShowCount = cntKaizen;

            //Sum CR Price
            var sumCRPrice = Context.View_Kaizens.Where(m => m.Date >= StartingDate && m.Date <= EndingDate && (m.KzStatus.Equals("Completed") || m.KzStatus.Equals("Award"))).Sum(s => s.ChkCost);
            string FormattedsumCRPrice = sumCRPrice.ToString("N");
            ViewBag.ShowCR = FormattedsumCRPrice;

            //Sum Award
            var sumPrice = Context.View_Kaizens.Where(m => m.Date >= StartingDate && m.Date <= EndingDate && (m.KzStatus.Equals("Completed") || m.KzStatus.Equals("Award"))).Sum(s => s.Price);
            string FormattedsumPrice = sumPrice.ToString("N");
            ViewBag.ShowPrice = FormattedsumPrice;

            return View("Dashboard");
        }

        public IActionResult UserProfile()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int intSessionEmpNumRow = Convert.ToInt32(HttpContext.Session.GetString(SessionEmpNumRow));

            if (intSessionEmpNumRow == 0)
            {
                return NotFound();
            }

            //var User = Context.Employee.FirstOrDefaultAsync(m => m.EMP_ID == intSessionEmpId);
            IEnumerable<View_ApprovalFlow> User = Context.View_ApprovalFlow.Where(a => a.Num_Row.Equals(intSessionEmpNumRow)).ToList();
            //var User = Context.View_ApprovalFlow.Where(a => a.PRS_NO.Equals(intSessionEmpId)).FirstOrDefault();

            if (User == null)
            {
                return NotFound();
            }

            return View("UserProfile", User);

        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove(SessionEmpId);
            HttpContext.Session.Remove(SessionEmpName);
            HttpContext.Session.Remove(SessionEmpBirth);
            return RedirectToAction("Login", "Login");
        }

        private bool UserExists(int id)
        {
            return Context.Employee.Any(e => e.EmpNo == id);
        }



    }
}