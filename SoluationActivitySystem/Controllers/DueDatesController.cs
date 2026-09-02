using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ActivitySystem.Data;
using ActivitySystem.Models;
using ActivitySystem.ViewModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ActivitySystem.Controllers
{
    public class DueDatesController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DueDatesController(DatabaseContext context)
        {
            _context = context;
        }

        const string SessionEmpNumRow = "_EmpNumRow";
        const string SessionEmpId = "_EmpId";
        const string SessionEmpName = "_EmpName";
        const string SessionEmpBirth = "_EmpBirth";
        const string SessionEmpAdmin = "_EmpAdmin";
        const string SessionEmpLeader = "_EmpLeader";
        const string SessionEmpManager = "_EmpManager";

        //Check Session
        public ActionResult Session()
        {
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId != null)
            {
                ViewBag.NumRow = HttpContext.Session.GetString(SessionEmpNumRow);
                ViewBag.Id = HttpContext.Session.GetString(SessionEmpId);
                ViewBag.Name = HttpContext.Session.GetString(SessionEmpName);
                ViewBag.Birth = HttpContext.Session.GetString(SessionEmpBirth);
                ViewBag.Admin = HttpContext.Session.GetString(SessionEmpAdmin);
                ViewBag.Leader = HttpContext.Session.GetString(SessionEmpLeader);
                ViewBag.Manager = HttpContext.Session.GetString(SessionEmpManager);
                return RedirectToAction("UserProfile", "Login");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        // GET: DueDates
        public async Task<IActionResult> Index()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(await _context.DueDate.OrderByDescending(i => i.DueDateNo).ToListAsync());
        }

        // GET: DueDates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var dueDate = await _context.DueDate
                .FirstOrDefaultAsync(m => m.DueDateNo == id);
            if (dueDate == null)
            {
                return NotFound();
            }

            return View(dueDate);
        }

        // GET: DueDates/Create
        public IActionResult Create()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View("Create");
        }

        // POST: DueDates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DueDateNo,StartingDate,EndingDate")] DueDate dueDate)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(dueDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dueDate);
        }

        // GET: DueDates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var dueDate = await _context.DueDate.FindAsync(id);
            if (dueDate == null)
            {
                return NotFound();
            }
            return View(dueDate);
        }

        // POST: DueDates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DueDateNo,StartingDate,EndingDate")] DueDate dueDate)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id != dueDate.DueDateNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dueDate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DueDateExists(dueDate.DueDateNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dueDate);
        }

        // GET: DueDates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var dueDate = await _context.DueDate
                .FirstOrDefaultAsync(m => m.DueDateNo == id);
            if (dueDate == null)
            {
                return NotFound();
            }

            return View(dueDate);
        }

        // POST: DueDates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dueDate = await _context.DueDate.FindAsync(id);
            _context.DueDate.Remove(dueDate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DueDateExists(int id)
        {
            return _context.DueDate.Any(e => e.DueDateNo == id);
        }
    }
}
