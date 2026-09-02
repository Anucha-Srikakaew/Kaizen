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
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace ActivitySystem.Controllers
{
    public class KaizensController : Controller
    {
        ILogger<LoginController> _logger;
        private readonly DatabaseContext _context;
        //private readonly IWebHostEnvironment _hostEnvironment;

       //public DatabaseContext _context;

        //private readonly IHostingEnvironment Env;IHostingEnvironment env

        //public KaizensController(DatabaseContext context, IWebHostEnvironment hostEnvironment)
        public KaizensController(ILogger<LoginController> logger, DatabaseContext context)
        {
            _context = context;
            //this._hostEnvironment = hostEnvironment;
            //Env = env; 
            _logger = logger;
        }

        const string SessionEmpNumRow   = "_EmpNumRow";
        const string SessionEmpId       = "_EmpId";
        const string SessionEmpName     = "_EmpName";
        const string SessionEmpBirth    = "_EmpBirth";
        const string SessionEmpAdmin    = "_EmpAdmin";
        const string SessionEmpLeader   = "_EmpLeader";
        const string SessionEmpManager  = "_EmpManager";


        //Check Session
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
                //return View("Login", "Login");
                return RedirectToAction("Login", "Login");
            }

        }

        //Get all Employee
        // GET: Kaizens
        //public async Task<IActionResult> Index(int? searchNo)
        public ActionResult Index(int? searchNo)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var varDueDate = _context.DueDate.Select(s => new SelectListItem
            {
                Value = s.DueDateNo.ToString(),
                Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
            }).OrderByDescending(r => r.Value).ToList();

            ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
            ViewBag.Search = "--- แสดงผลทั้งหมด ---";
            ViewBag.CntKaizen = "-";
            ViewBag.SumPrice = "-";
            ViewBag.SumPrice = "-";

            
            //int empId;
            //if (!int.TryParse(strSessionEmpId, out empId))
            //{
            //    // Handle invalid or null session value here
            //    empId = Convert.ToInt32(strSessionEmpId);
            //}

            //var view_kaizen_index = _context.View_Kaizens
            //                        .Where(m => m.IdEmp.HasValue && m.IdEmp.Value == empId)
            //                        .OrderByDescending(i => i.KaizenNo)
            //                        .ToList();

            //int intSessionEmpId = Convert.ToInt32(strSessionEmpId);

            //var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId))).OrderByDescending(i => i.KaizenNo).ToList();


            var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus != "Cancelled")).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
            if (searchNo != null)
            {
                var getDueDate          = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                DateTime StartingDate   = getDueDate.StartingDate;
                DateTime EndingDate     = getDueDate.EndingDate;

                view_kaizen_index = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus != "Cancelled")  && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).ToList();

                //Show Search
                ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                //Cnt
                var cntKaizen  = view_kaizen_index.Count();
                ViewBag.CntKaizen = cntKaizen;

                //Sum Price
                var sumPrice   = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId)) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                string FormattedsumPrice = sumPrice.ToString("N");
                ViewBag.SumPrice = FormattedsumPrice;

                //Sum CR Price
                var sumCRPrice = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId)) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                string FormattedsumCRPrice = sumCRPrice.ToString("N");
                ViewBag.SumCRPrice = FormattedsumCRPrice;
            }

            return View("Index",view_kaizen_index);

            //return View(_context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId))).OrderByDescending(i => i.KaizenNo).ToList());

            //return View(await _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt16(strSessionEmpId))).OrderByDescending(i => i.KaizenNo).ToListAsync());
        }

        //Get all Employee
        // GET: Kaizens/Details/5
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
        
            var kaizen = await _context.View_Kaizens.DefaultIfEmpty()
                .FirstOrDefaultAsync(m => m.KaizenNo == id);
            if (kaizen == null)
            {
                return NotFound();
            }

            return View(kaizen);
        }

        //Get all Employee
        public async Task<IActionResult> LibraryDetails(int? id)
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

            var view_kaizen = await _context.View_Kaizens.DefaultIfEmpty()
                .FirstOrDefaultAsync(m => m.KaizenNo == id);
            if (view_kaizen == null)
            {
                return NotFound();
            }
            return View(view_kaizen);
        }

        //By Employee Leader
        // GET: Kaizens/Create
        public IActionResult Create()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            //var varDept = _context.Departments.Select(s => new SelectListItem
            //              {
            //                 Value = s.DeptNo.ToString(),
            //                 Text = s.Department + " - " + s.Section
            //              }).ToList();

            //ViewBag.Dept = new SelectList(varDept, "Value", "Text");


            ViewBag.DateNow = DateTime.Now.ToString("dd/MM/yyyy"); 

             //IEnumerable<ActType_Choose> listActType = _context.ActType_Choose.Select(d => new SelectListItem
             //                            {
             //                                Value = d.NoActType.ToString(),
             //                                Text = d.ActTypeTh
             //                            }
             //                            );

             //ViewBag.Domains = listActType;

             var varActType = _context.ActType_Choose.Select(s => new SelectListItem
            {
                Value = s.NoActType.ToString(),
                Text = s.ActTypeTh + " " + s.ActTypeEng
            }).ToList();

            ViewBag.ActType = new SelectList(varActType, "Value", "Text");

            //var ActTypeList = (from s in _context.ActType_Choose
            //                    select new SelectListItem()
            //                   {
            //                        Value = s.NoActType.ToString(),
            //                        Text = s.ActTypeTh + " " + s.ActTypeEng
            //                    }).ToList();

            //ActTypeList.Insert(0, new SelectListItem()
            //{
            //    Text = "----Select----",
            //    Value = string.Empty
            //});

            //ViewBag.ActType = ActTypeList;

            return View();

        }

        //By Employee Leader
        // POST: Kaizens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("KaizenNo,Title,Date,NoActType,ActDetail,ImpBefore,Problem,ImpAfter,Result,CostSaving")] Kaizen kaizen, View_Kaizens view_kaizen)
        public async Task<IActionResult> Create([Bind("Title,Date,NoActType,ActDetail,ImageBefore,Problem,ImageAfter,Result,CostSaving")] Kaizen kaizen, Library library, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            //if (ModelState.IsValid)m
            //{

            //var insuranceKaizenRef = new Kaizen
            //{
            //    KaizenRef = "",
            //};
            //_context.Add(insuranceKaizenRef);
            //_context.Update(insuranceKaizenRef);
            string DocKaizen = "";

                if (strSessionEmpNumRow != null)
                {

                        //var view_kaizen = await _context.View_Kaizens.FirstOrDefault();
                        //if (view_kaizen == null)
                        //{
                        //    return NotFound();
                        //}
                        //Get Imp Before and After
                        //string uniqueFileNameBefore = UploadedFileBefore(view_kaizen);
                        //string uniqueFileNameAfter  = UploadedFileAfter(view_kaizen);

                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();
                        string sDeptNo = getDeptNo.DEPT_CODE.Substring(0, 4);

                        //Get Running Code Year
                        string runningCodeYear = DateTime.Now.ToString("yy");

                        //Get Running Code Number
                        //var runningCode = await _context.Kaizen
                        //.FirstOrDefaultAsync(m => m.DeptNo == DeptNo);

                        var runningNumber = _context.Kaizen.Where(m => m.DeptNo.Equals(sDeptNo)).Count();

                        int runningCodeNo   = runningNumber + 1;

                        //string runningCodeNoStr = "0000";

                        //if(runningCodeNo > 0 & runningCodeNo < 10)
                        //{
                        //    runningCodeNoStr = "000" + runningCodeNo;
                        //} else if (runningCodeNo >= 10 & runningCodeNo < 100)
                        //{
                        //    runningCodeNoStr = "00" + runningCodeNo;
                        //} else if (runningCodeNo >= 100 & runningCodeNo < 1000)
                        //{
                        //    runningCodeNoStr = "0" + runningCodeNo;
                        //} else if (runningCodeNo >= 1000 & runningCodeNo < 10000)
                        //{
                        //    runningCodeNoStr = runningCodeNo.ToString();
                        //}

                        string runningCodeNoStr = runningCodeNo.ToString("D4");

                        //Input Data to model
                        kaizen.DeptNo    = sDeptNo;
                        DocKaizen = "K" + sDeptNo + runningCodeYear + "-" + runningCodeNoStr;
                        kaizen.KaizenRef = DocKaizen;
                        kaizen.KzStatus  = "NewIdea";
                        kaizen.Date      = getDatetime;

                        //kaizen.ImpBefore    = uniqueFileNameBefore;
                        //kaizen.ImpAfter     = uniqueFileNameAfter;

                        //Path Image
                        //string wwwRootPath = this.Env.WebRootPath;
                        //string contentPath = this.Env.ContentRootPath;

                        //string fileNameBefore = Path.GetFileNameWithoutExtension(kaizen.ImageBefore.FileName);
                        //string fileNameAfter = Path.GetFileNameWithoutExtension(kaizen.ImageAfter.FileName);

                        //if (kaizen.ImageBefore != null)
                        // Handle ImageBefore upload
                        if (kaizen.ImageBefore != null && kaizen.ImageBefore.Length > 0)
                        {
                                string extensionNameBefore = Path.GetExtension(kaizen.ImageBefore.FileName);
                                string fileNameBefore = DocKaizen + "_Before" + extensionNameBefore;

                                string uploadsBefore = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameBefore);
                                using (var fileStreamBefore = new FileStream(uploadsBefore, FileMode.Create))
                                {
                                        await kaizen.ImageBefore.CopyToAsync(fileStreamBefore);
                                        kaizen.ImpBefore = fileNameBefore;
                                }
                        }
                        else
                        {
                                kaizen.ImpBefore = "Empty.jpg";
                        }

                        if (kaizen.ImageAfter != null && kaizen.ImageAfter.Length > 0)
                {
                               string extensionNameAfter  = Path.GetExtension(kaizen.ImageAfter.FileName);
                               string fileNameAfter = DocKaizen + "_After" + extensionNameAfter;
                               
                               string uploadsAfter  = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameAfter);
                               using (var fileStreamAfter = new FileStream(uploadsAfter, FileMode.Create))
                               {
                                        await kaizen.ImageAfter.CopyToAsync(fileStreamAfter);
                                        kaizen.ImpAfter  = fileNameAfter;
                               }
                        }
                        else
                        {
                            kaizen.ImpAfter = "Empty.jpg";
                        }

                        //Select find Leader First Level
                        var flow_leader     = _context.View_ApprovalFlow.Where(a => a.PRS_NO.Equals(getDeptNo.PRS_NO) && a.EDS_APV_TYPE.Equals(2)).OrderBy(i => i.EDS_LEVEL).DefaultIfEmpty().FirstOrDefault();

                        //Select find Leader First Level
                        var flow_manager    = _context.View_ApprovalFlow.Where(a => a.PRS_NO.Equals(getDeptNo.PRS_NO) && a.EDS_APV_TYPE.Equals(2)).OrderByDescending(i => i.EDS_LEVEL).DefaultIfEmpty().FirstOrDefault();

                        //Insert Table Library
                        //var library = _context.Library.FirstOrDefault();
                        library.KaizenRef   = DocKaizen;
                        var getActType      = _context.ActType_Choose.Where(m => m.NoActType.Equals(kaizen.NoActType)).DefaultIfEmpty().FirstOrDefault();
                        library.ChkActType  = getActType.ActTypeTh + "  ("+ getActType.ActTypeEng  + ")";
                        library.DateEmp     = getDatetime;
                        library.IdEmp       = getDeptNo.PRS_NO;
                        //library.NameEmp = getDeptNo.EMP_E_NAME;
                        library.NameEmp     = getDeptNo.EMP_INTL + " " + getDeptNo.EMP_NAME + " " + getDeptNo.EMP_SURNME;
                        library.DeptEmp     = getDeptNo.DEPT_ENGDESC;
                        library.JbtEmp      = getDeptNo.JBT_ENGDESC;
                        library.LibraryStatus = "InProcess";

                        //Temp Insert Table Library
                        library.DateLeader = kaizen.Date;
                        //Leader
                        library.IdLeader = flow_leader.Approval_No;
                        library.NameLeader = "คุณ" + flow_leader.Approval_Name;
                        library.DeptLeader = flow_leader.Approval_DeptCode;
                        library.JbtLeader = flow_leader.Approval_Jbt;
                        library.NoLevel = 0;
                        library.NoType = 0;
                        library.NoEnv = 0;
                        library.ChkCost = 0;
                        library.AwardNo = 0;

                        //Manager Edit Fix Code TOS
                        if (flow_manager.Approval_DeptCode == "OT01")
                        {   
                            //กำหนดค่าชั่วคราว 10/2/22
                            library.DateManager = kaizen.Date;
                            library.IdManager   = 96111;
                            library.NameManager = "คุณพรชัย คงตะแบก";
                            library.DeptManager = "PD03";
                            library.JbtManager  = "(PD) Manager";
                        }
                        //else
                        ////Manager Edit Fix Code SHE
                        //if (flow_manager.Approval_DeptCode == "MN01")
                        //{
                        //    //กำหนดค่าชั่วคราว 21/4/22
                        //    library.DateManager = kaizen.Date;
                        //    library.IdManager = 96584;
                        //    library.NameManager = "คุณภัทนิดา ทวีเจริญพันธ์";
                        //    library.DeptManager = "MN01";
                        //    library.JbtManager = "Management";
                        //}
                        else
                        {
                                library.DateManager = kaizen.Date;
                                library.IdManager = flow_manager.Approval_No;
                                library.NameManager = "คุณ" + flow_manager.Approval_Name;
                                library.DeptManager = flow_manager.Approval_DeptCode;
                                library.JbtManager = flow_manager.Approval_Jbt;
                        }


                        //Admin (SGA Bord)
                        library.DateSGABoard = kaizen.Date;
                        library.IdSGABoard = 0;
                        library.Price = 0;

                        //Process
                        process.KaizenRef = DocKaizen;
                        process.Permission = "Employee";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Created";

                        _context.Add(kaizen);
                        _context.Add(library);
                        _context.Add(process);

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                }

            return View(kaizen);
    }

        //By Employee
        // GET: Kaizens/Edit/5
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

            var varActType = _context.ActType_Choose.Select(s => new SelectListItem
            {
                Value = s.NoActType.ToString(),
                Text = s.ActTypeTh + " " + s.ActTypeEng
            }).ToList();

            ViewBag.ActType = new SelectList(varActType, "Value", "Text");

			//var kaizen1 = _context.Kaizen.DefaultIfEmpty();
			//var kaizen = _context.Kaizen.Where(k => k.KaizenNo == id).DefaultIfEmpty();

			//var kaizen = await _context.Kaizen.FindAsync(id);
			var kaizen = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			if (kaizen == null)
            {
                return NotFound();
            }
            return View(kaizen);
        }

        //By Employee
        // POST: Kaizens/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KaizenNo, DeptNo,KaizenRef,Title,Date,NoActType,ActDetail,ImageBefore,ImpBefore, Problem,ImageAfter,ImpAfter,Result,CostSaving,KzStatus")] Kaizen kaizen, Library library, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            //if (id != kaizen.KaizenNo)
            //{
            //    return NotFound();
            //}

            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_update = await _context.Kaizen.FindAsync(id);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_update.KzStatus == "NewIdea")
                    {

						if (kaizen.ImageBefore != null && kaizen.ImageBefore.Length > 0)
						{
							string extensionNameBefore = Path.GetExtension(kaizen.ImageBefore.FileName);
							string fileNameBefore = kaizen.KaizenRef + "_Before" + extensionNameBefore;

							string uploadsBefore = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameBefore);
							using (var fileStreamBefore = new FileStream(uploadsBefore, FileMode.Create))
							{
								await kaizen.ImageBefore.CopyToAsync(fileStreamBefore);
								kaizen_update.ImpBefore = fileNameBefore;
							}
						}
						//else
						//{
      //                      if (kaizen.ImpBefore == null)
      //                      {
      //                          kaizen_update.ImpBefore = "Empty.jpg";
      //                      }
						//}

						if (kaizen.ImageAfter != null && kaizen.ImageAfter.Length > 0)
						{
							string extensionNameAfter = Path.GetExtension(kaizen.ImageAfter.FileName);
							string fileNameAfter = kaizen.KaizenRef + "_After" + extensionNameAfter;

							string uploadsAfter = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameAfter);
							using (var fileStreamAfter = new FileStream(uploadsAfter, FileMode.Create))
							{
								await kaizen.ImageAfter.CopyToAsync(fileStreamAfter);
								kaizen_update.ImpAfter = fileNameAfter;
							}
						}
						//else
						//{
      //                      if (kaizen.ImageAfter == null)
      //                      {
						//		kaizen_update.ImpAfter = "Empty.jpg";
						//	}

						//}

						//if (kaizen.ImageBefore != null & kaizen.ImpBefore != "Empty.jpg")
      //                  {
      //                      string extensionNameBefore = Path.GetExtension(kaizen.ImageBefore.FileName);
      //                      string fileNameBefore = kaizen.KaizenRef + "_Before" + extensionNameBefore;

      //                      string uploadsBefore = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameBefore);
      //                      using (var fileStreamBefore = new FileStream(uploadsBefore, FileMode.Create))
      //                      {
      //                          await kaizen.ImageBefore.CopyToAsync(fileStreamBefore);
      //                          kaizen_update.ImpBefore = fileNameBefore;
      //                      }

      //                  }
      //                  else
      //                  {
      //                      kaizen_update.ImpBefore = kaizen.ImpBefore;
      //                  }

      //                  if (kaizen.ImageAfter != null & kaizen.ImpBefore != "Empty.jpg")
      //                  {
      //                      string extensionNameAfter = Path.GetExtension(kaizen.ImageAfter.FileName);
      //                      string fileNameAfter = kaizen.KaizenRef + "_After" + extensionNameAfter;

      //                      string uploadsAfter = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", fileNameAfter);
      //                      using (var fileStreamAfter = new FileStream(uploadsAfter, FileMode.Create))
      //                      {
      //                          await kaizen.ImageAfter.CopyToAsync(fileStreamAfter);
      //                          kaizen_update.ImpAfter = fileNameAfter;
      //                      }
      //                  }
      //                  else
      //                  {
      //                      kaizen_update.ImpAfter = kaizen.ImpAfter;
      //                  }
                       


                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;

                        //After update auto datesave at Create Date
                        kaizen_update.Date = getDatetime;

                        kaizen_update.Title = kaizen.Title;
                        kaizen_update.NoActType = kaizen.NoActType;
                        kaizen_update.ActDetail = kaizen.ActDetail;
                        kaizen_update.Problem = kaizen.Problem;
                        kaizen_update.Result = kaizen.Result;
                        kaizen_update.CostSaving = kaizen.CostSaving;

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen.KaizenRef;
                        process.Permission = "Employee";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Edited";

                        _context.Update(kaizen_update);
                        //_context.Update(library_update);
                        _context.Add(process);


                        await UpdateLibrary(kaizen_update, id);


                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to history!!";
                        return View(kaizen);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(kaizen.KaizenNo))
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
            return View(kaizen);
        }

        //By Employee
        public async Task<IActionResult> UpdateLibrary(Kaizen kaizen, int id)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(id);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			//Update Table Library
			var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            var getActType = _context.ActType_Choose.Where(m => m.NoActType.Equals(kaizen.NoActType)).DefaultIfEmpty().FirstOrDefault();
            library_update.ChkActType = getActType.ActTypeTh + "  (" + getActType.ActTypeEng + ")";
            library_update.DateEmp = kaizen.Date;

            _context.Update(library_update);
            return View(kaizen);
        }

        //By Employee
        // GET: Kaizens/Edit/5
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

            //var kaizen = await _context.View_Kaizens.FindAsync(id);
			var kaizen = await _context.View_Kaizens.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			if (kaizen == null)
            {
                return NotFound();
            }
            return View(kaizen);
        }

        //By Employee
        // POST: Kaizens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("KaizenNo,DeptNo,KaizenRef,Title,Date,NoActType,ActDetail,ImpBefore,Problem,ImpAfter,Result,CostSaving,KzStatus")] View_Kaizens kaizen, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (id != kaizen.KaizenNo)
            {
                return NotFound();
            }

            if (strSessionEmpNumRow != null)
            {
                try
                {

                    //var kaizen_update = await _context.Kaizen.FindAsync(id);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_update.KzStatus == "NewIdea")
                    {
                        //Not Delete but Change Status to History
                  


                        if (kaizen_update.ImpBefore != null & kaizen_update.ImpBefore != "Empty.jpg")
                        {
                            //string extensionNameBefore = Path.GetExtension(kaizen_update.ImageBefore.FileName);
                            //string fileNameBefore = kaizen.KaizenRef + "_Before" + extensionNameBefore;

                            string uploadsBefore = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", kaizen_update.ImpBefore);
                            FileInfo fileBefore = new FileInfo(uploadsBefore);
                            if (fileBefore != null)
                            {
                                System.IO.File.Delete(uploadsBefore);
                                fileBefore.Delete();
                                kaizen_update.ImpBefore = "Empty.jpg";
                            }
                            //using (var fileStreamBefore = new FileStream(uploadsBefore, FileMode.Create))
                            //{
                            //    //await kaizen_update.ImageBefore.CopyToAsync(fileStreamBefore);

                            //    kaizen.ImpBefore = "NULL";
                            //}

                        }

                        if (kaizen_update.ImpAfter != null & kaizen_update.ImpBefore != "Empty.jpg")
                        {                                                                                                      
                            //string extensionNameAfter = Path.GetExtension(kaizen_update.ImageAfter.FileName);
                            //string fileNameAfter = kaizen.KaizenRef + "_After" + extensionNameAfter;

                            string uploadsAfter = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesUploaded", kaizen_update.ImpAfter);
                            FileInfo fileAfter = new FileInfo(uploadsAfter);
                            if (fileAfter != null)
                            {
                                System.IO.File.Delete(uploadsAfter);
                                fileAfter.Delete();
                                kaizen_update.ImpAfter = "Empty.jpg";
                            }
                            //using (var fileStreamAfter = new FileStream(uploadsAfter, FileMode.Create))
                            //{
                            //    //await kaizen_update.ImageAfter.CopyToAsync(fileStreamAfter);
                            //    kaizen.ImpAfter = "NULL";
                            //}
                        }

                        kaizen_update.KzStatus = "Cancelled";

                        ////Update Table Library
                        //var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).FirstOrDefault();

                        //library_update.LibraryStatus = "History";

                        //Get Datetime
                        DateTime getDatetime    = DateTime.Now;
                        //Get Dept. Emp
                        var getDeptNo           = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen_update.KaizenRef;
                        process.Permission = "Employee";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Cancelled";

                        _context.Update(kaizen_update);
                        //_context.Update(library_update);
                        _context.Add(process);

                        await DeleteLibrary(kaizen_update, id);

                        await _context.SaveChangesAsync();

                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "Update successfully!!";
                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to history!!";
                        return View(kaizen);
                    }

                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(kaizen.KaizenNo))
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

            return View(kaizen);
        }

        //By Employee
        public async Task<IActionResult> DeleteLibrary(Kaizen kaizen, int id)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(id);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			//var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).FirstOrDefault();
			var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
			library_update.LibraryStatus = "Cancelled";

            _context.Update(library_update);
            return View(kaizen);
        }

        //By Employee
        // GET: Kaizens/Edit/5
        public async Task<IActionResult> SendApproval(int? id)
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

            //var kaizen = await _context.View_Kaizens.FindAsync(id);
			var kaizen = await _context.View_Kaizens.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			if (kaizen == null)
            {
                return NotFound();
            }
            return View(kaizen);
        }


        //By Employee
        // POST: Kaizens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendApproval(int id, [Bind("KaizenNo,DeptNo,KaizenRef,Title,Date,NoActType,ActDetail,ImpBefore,Problem,ImpAfter,Result,CostSaving,KzStatus")] View_Kaizens kaizen, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (id != kaizen.KaizenNo)
            {
                return NotFound();
            }

            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_send = await _context.Kaizen.FindAsync(id);
					var kaizen_send = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
					if (kaizen_send == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_send.KzStatus == "NewIdea")
                    {  
                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;

                        //Update Status Approval to Leader
                        //String sDate  = DateTime.Now.ToString("dd/MM/yyyy");
                        kaizen_send.Date     = getDatetime;
                        kaizen_send.KzStatus = "WaitLeader";

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen_send.KaizenRef;
                        process.Permission = "Employee";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Sended";

                        _context.Update(kaizen_send);
                        _context.Add(process);
                        await SendApprovalLibrary(kaizen_send, id);

                        await _context.SaveChangesAsync();

                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "ส่งเรื่องอนุมัติเรียบร้อยแล้ว กรุณาเช็คสถานะเอกสารอีกครั้ง (Send to Approval Successfully!!)";

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "ไม่สามารถส่งเรื่องอนุมัติได้ กรุณาตรวจสอบข้อมูลอีกครั้ง หรือติดต่อแผนกไอที (Status not ready to send!!)";
                        return View(kaizen);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(kaizen.KaizenNo))
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
            return View(kaizen);
        }

        //RejectByLeader
        public async Task<IActionResult> Reject(int? id)
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

            //var kaizen = await _context.View_Kaizens.FindAsync(id);
            var kaizen = await _context.View_Kaizens.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
            if (kaizen == null)
            {
                return NotFound();
            }
            return View(kaizen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, [Bind("KaizenNo,DeptNo,KaizenRef,Title,Date,NoActType,ActDetail,ImpBefore,Problem,ImpAfter,Result,CostSaving,KzStatus")] View_Kaizens kaizen, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (id != kaizen.KaizenNo)
            {
                return NotFound();
            }

            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_send = await _context.Kaizen.FindAsync(id);
                    var kaizen_send = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
                    if (kaizen_send == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_send.KzStatus == "WaitLeader" || kaizen_send.KzStatus == "WaitManager")
                    {
                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;

                        //Update Status Approval to Leader
                        //String sDate  = DateTime.Now.ToString("dd/MM/yyyy");
                        kaizen_send.Date = getDatetime;
                        kaizen_send.KzStatus = "NewKaizen";

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen_send.KaizenRef;
                        process.Permission = "Leader";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Reject";

                        _context.Update(kaizen_send);
                        _context.Add(process);
                        await RejectLibrary(kaizen_send, id);

                        await _context.SaveChangesAsync();

                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "ตีกลับเอกสารเรียบร้อยแล้ว กรุณาเช็คสถานะเอกสารอีกครั้ง (Reject to Approval Successfully!!)";

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "ไม่สามารถตีกลับเอกสารได้ กรุณาตรวจสอบข้อมูลอีกครั้ง หรือติดต่อแผนกไอที (Status not ready to send!!)";
                        return View(kaizen);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(kaizen.KaizenNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Leader", "Kaizens", kaizen);
            }
            return RedirectToAction("Leader", "Kaizens", kaizen);
        }

        //By Leader
        public async Task<IActionResult> RejectLibrary(Kaizen kaizen, int id)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(id);
            var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
            var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            library_update.ChkImproved = "";
            library_update.ChkStandard = "";
            library_update.ChkStandardRef = "";
            library_update.ChkDept = "";
            library_update.ChkDeptRef = "";
            library_update.NoLevel =0;
            library_update.ChkLevel = "";
            library_update.NoType = 0;
            library_update.ChkType = "";
            library_update.ChkCost = 0;
            library_update.NoEnv = 0;
            library_update.ChkEnv = "";
            library_update.RemarkLeader = "";
            library_update.LibraryStatus = "Leader";

            _context.Update(library_update);
            return RedirectToAction("Leader", "Kaizens", kaizen);
        }


        //By Employee
        public async Task<IActionResult> SendApprovalLibrary(Kaizen kaizen, int id)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(id);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            library_update.LibraryStatus = "User";

            _context.Update(library_update);
            return View(kaizen);
        }

        //Show All
        public async Task<IActionResult> Library(int? searchNo, string searchDept)
        {
            //Session();
            //string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);

            //return View(_context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea").OrderByDescending(i => i.KaizenNo).ToList());

            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Page = "Library";

            if (searchNo == 0)
            {
                searchNo = null;
            }

            if (searchDept == "--- แสดงผลทั้งหมด ---")
            {
                searchDept = null;
            }

            var varDueDate = _context.DueDate.Select(s => new SelectListItem
            {
                Value = s.DueDateNo.ToString(),
                Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
            }).OrderByDescending(r => r.Value).ToList();

            ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
            ViewBag.Search      = "--- แสดงผลทั้งหมด ---";
            ViewBag.CntKaizen   = "-";
            ViewBag.SumPrice    = "-";
            ViewBag.SumPrice    = "-";

            var varDeptTab = _context.View_GrpDeptTab.Select(s => new SelectListItem
            {
                Value = s.DEPT_CODE.ToString(),
                Text  = s.DEPT_CODE + ": " + s.DEPT_ENGDESC + " " + s.DEPT_THAIDESC
            }).OrderBy(r => r.Value).ToList();

            ViewBag.DeptTab = new SelectList(varDeptTab, "Value", "Text");

            //1 searchNo == null && searchDept == null
            var view_kaizen_library = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea").OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

            //2 searchNo != null && searchDept == null
            if (searchNo != null && searchDept == null)
            {
                var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                DateTime StartingDate = getDueDate.StartingDate;
                DateTime EndingDate   = getDueDate.EndingDate;

                view_kaizen_library = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                //Show Search
                ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                //Cnt
                var cntKaizen = view_kaizen_library.Count();
                ViewBag.CntKaizen = cntKaizen;

                //Sum Price
                var sumPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                string FormattedsumPrice = sumPrice.ToString("N");
                ViewBag.SumPrice = FormattedsumPrice;

                //Sum CR Price
                var sumCRPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                string FormattedsumCRPrice = sumCRPrice.ToString("N");
                ViewBag.SumCRPrice = FormattedsumCRPrice;
            }

            //3 searchNo == null && searchDept != null
            if (searchNo == null && searchDept != null )
            {
                var getDeptNo   = _context.View_GrpDeptTab.Where(d => d.DEPT_CODE == searchDept).DefaultIfEmpty().FirstOrDefault();
                String DEPT_TH  = getDeptNo.DEPT_THAIDESC;
                String Dept_ENG = getDeptNo.DEPT_ENGDESC;
                string sDeptNo = getDeptNo.DEPT_CODE;
                //string sDeptNo = searchDept.Substring(0, 4);

                view_kaizen_library = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.Equals(sDeptNo)).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                //Show Search
                ViewBag.Search = "แผนก " + Dept_ENG + " " + DEPT_TH;

                //Cnt
                var cntKaizen = view_kaizen_library.Count();
                ViewBag.CntKaizen = cntKaizen;

                //Sum Price
                var sumPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.Equals(sDeptNo)).Sum(s => s.Price);
                string FormattedsumPrice = sumPrice.ToString("N");
                ViewBag.SumPrice = FormattedsumPrice;

                //Sum CR Price
                var sumCRPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.Equals(sDeptNo)).Sum(s => s.ChkCost);
                string FormattedsumCRPrice = sumCRPrice.ToString("N");
                ViewBag.SumCRPrice = FormattedsumCRPrice;
            }
            //4 searchNo != null && searchDept != null
            if (searchNo != null && searchDept != null)
            {
                var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                DateTime StartingDate = getDueDate.StartingDate;
                DateTime EndingDate = getDueDate.EndingDate;

                var getDeptNo = _context.View_GrpDeptTab.Where(d => d.DEPT_CODE == searchDept).DefaultIfEmpty().FirstOrDefault();
                String DEPT_TH = getDeptNo.DEPT_THAIDESC;
                String Dept_ENG = getDeptNo.DEPT_ENGDESC;
                string sDeptNo = searchDept.Substring(0, 4);

                view_kaizen_library = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.StartsWith(sDeptNo) && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                //Show Search
                ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy") + " ของแผนก " + Dept_ENG + " " + DEPT_TH;

                //Cnt
                var cntKaizen = view_kaizen_library.Count();
                ViewBag.CntKaizen = cntKaizen;

                //Sum Price
                var sumPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.StartsWith(sDeptNo) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                string FormattedsumPrice = sumPrice.ToString("N");
                ViewBag.SumPrice = FormattedsumPrice;

                //Sum CR Price
                var sumCRPrice = _context.View_Kaizens.Where(m => m.KzStatus != "Cancelled" && m.KzStatus != "NewIdea" && m.DeptNo.StartsWith(sDeptNo) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                string FormattedsumCRPrice = sumCRPrice.ToString("N");
                ViewBag.SumCRPrice = FormattedsumCRPrice;
            }

            searchNo    = null;
            searchDept  = null;

            return View("Library", view_kaizen_library);

        }

        //By Leader
        public async Task<IActionResult> Leader(int? searchNo)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionLeader = HttpContext.Session.GetString(SessionEmpLeader);
            if (strSessionLeader == "Leader")
            {
                var varDueDate = _context.DueDate.Select(s => new SelectListItem
                {
                    Value = s.DueDateNo.ToString(),
                    Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
                }).OrderByDescending(r => r.Value).ToList();

                ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
                ViewBag.Search = "--- แสดงผลทั้งหมด ---";
                ViewBag.CntKaizen = "-";
                ViewBag.SumPrice = "-";
                ViewBag.SumPrice = "-";

                var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdLeader.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus.Equals("WaitLeader") || m.KzStatus.Equals("WaitManager")) ).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
        

                if (searchNo != null)
                {
                    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                    DateTime StartingDate = getDueDate.StartingDate;
                    DateTime EndingDate   = getDueDate.EndingDate;

                    view_kaizen_index = _context.View_Kaizens.Where(m => m.IdLeader.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus.Equals("WaitLeader") || m.KzStatus.Equals("WaitManager")) && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).ToList();

                    //Show Search
                    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                    //Cnt
                    var cntKaizen = view_kaizen_index.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => m.IdLeader.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus.Equals("WaitLeader") || m.KzStatus.Equals("WaitManager")) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;
                     
                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => m.IdLeader.Equals(Convert.ToInt32(strSessionEmpId)) && (m.KzStatus.Equals("WaitLeader") || m.KzStatus.Equals("WaitManager")) && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }


                // Check if the list is empty before passing to view
                if (view_kaizen_index == null || !view_kaizen_index.Any())
                {
                    view_kaizen_index = new List<View_Kaizens>();
                }
               //if (view_kaizen_index[0] == null)
                //{
                //    view_kaizen_index = new List<View_Kaizens>();
                //}

                return View(view_kaizen_index);

                //return View("Leader",_context.View_Kaizens.Where(m => m.IdLeader.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitLeader")).OrderByDescending(i => i.KaizenNo).ToList());
            
            }
            else
            {
               return View("Index");
            }
        }

        public async Task<IActionResult> Manager()
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            string strSessionManager = HttpContext.Session.GetString(SessionEmpManager);
            if (strSessionManager == "Manager")
            {
                List<SelectListItem> KaizenList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem {
                    Text = "No", Value = "2"
                },};
                ViewBag.CntKaizen = new SelectList(KaizenList, "Value", "Text");

                List<SelectListItem> CRList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem {
                    Text = "No", Value = "2"
                },};
                ViewBag.CntCR = new SelectList(CRList, "Value", "Text");

                List<SelectListItem> VoteList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem {
                    Text = "No", Value = "2"
                },};
                ViewBag.CntVote = new SelectList(VoteList, "Value", "Text");

                return View("Manager", _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitManager")).OrderByDescending(i => i.KaizenNo).ToList());
            }
            else
            {
                return View("Index");
            }
        }

        //By Leader
        // GET: Kaizens/Check/5  For Leader Only
        public async Task<IActionResult> Check(int? id)
        {
            Session();
            if (id == null)
            {
                return NotFound();
            }

            List<SelectListItem> ImprovedList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Completed", Value = "1"
                },
                new SelectListItem {
                    Text = "Suggestion", Value = "2"
                },
            };
            ViewBag.ImprovedList = new SelectList(ImprovedList, "Value", "Text");

            List<SelectListItem> YesNoList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem {
                    Text = "No", Value = "2"
                },
            };
            ViewBag.YesNoList = new SelectList(YesNoList, "Value", "Text");

            List<SelectListItem> DeptList = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "Engineering", Value = "1"
                },
                new SelectListItem {
                    Text = "Facility", Value = "2"
                },
                 new SelectListItem {
                    Text = "Safety/Environment", Value = "3"
                },
                  new SelectListItem {
                    Text = "Other", Value = "4"
                },
            };
            ViewBag.DeptList = new SelectList(DeptList, "Value", "Text");

            var LevelKaizenList = _context.LevelKaizen_Choose.Select(s => new SelectListItem
            {
                Value = s.NoLevel.ToString(),
                Text = s.LevelTh + " " + s.LevelEng
            }).ToList();
            ViewBag.LevelKaizenList = new SelectList(LevelKaizenList, "Value", "Text");

            var EvaluateImpList = _context.TypeKaizen_Choose.Select(s => new SelectListItem
            {
                Value = s.NoType.ToString(),
                Text = s.TypeTh + " " + s.TypeEng
            }).ToList();
            ViewBag.EvaluateImpList = new SelectList(EvaluateImpList, "Value", "Text");

            var EnvList = _context.EnvKaizen_Choose.Select(s => new SelectListItem
            {
                Value = s.NoEnv.ToString(),
                Text = s.EnvTh + " " + s.EnvEng
            }).ToList();
            ViewBag.EnvList = new SelectList(EnvList, "Value", "Text");

            //var kaizen = await _context.Kaizen.FindAsync(id);
			var kaizen = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
			if (kaizen == null)
            {
                return NotFound();
            }

            var library = _context.Library.Where(a => a.KaizenRef.Equals(kaizen.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            if (library == null)
            {
                return NotFound();
            }

            var view_kaizen = _context.View_Kaizens.Where(a => a.KaizenRef.Equals(kaizen.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            if (view_kaizen == null)
            {
                return NotFound();
            }
            return View(view_kaizen); //View("Check", library);
        }

        //By Leader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(int id, [Bind("LibNo,KaizenRef,DateLeader,IdLeader,NameLeader,DeptLeader,JbtLeader,ChkImproved,ChkStandard,ChkStandardRef,ChkDept,ChkDeptRef,NoLevel,ChkLevel,NoType,ChkType,ChkCost,NoEnv, ChkEnv,RemarkLeader")] View_Kaizens view_kaizen, Library library, Process process)   
        {
            Session();
            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            //if (id != view_kaizen.KaizenNo)
            //{
            //    return NotFound();
            //}

            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_update = await _context.Kaizen.FindAsync(id);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == id);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }
             
                    if (kaizen_update.KzStatus == "WaitLeader")
                    {

                        ////Library
                        //var library_update = _context.Library.Where(a => a.KaizenRef.Equals(kaizen.KaizenRef)).FirstOrDefault();
                        //if (library_update == null)
                        //{
                        //    return NotFound();
                        //}

                        // Add Library
                        //library.ChkImproved = "";
                        //library.ChkStandard = "";
                        //library.ChkStandardRef = "";

                        //library.ChkDept = "";
                        //library.ChkDeptRef = "";

                        if (library.ChkImproved == "1")
                        {
                            library.ChkImproved = "Completed"; //New Request from QM afer table created
                        }
                        else if (library.ChkImproved == "2")
                        {
                            library.ChkImproved = "Suggestion";
                        }

                        if (library.ChkStandard == "1")
                        {
                            library.ChkStandard = "Yes";
                        }
                        else if (library.ChkStandard == "2")
                        {
                            library.ChkStandard = "No";
                        }

                        if (library.ChkDept == "1")
                        {
                            library.ChkDept = "Engineering";
                        }
                        else if (library.ChkDept == "2")
                        {
                            library.ChkDept = "Facility";
                        }
                        else if (library.ChkDept == "3")
                        {
                            library.ChkDept = "Safety/Environment";
                        }
                        else if (library.ChkDept == "4")
                        {
                            library.ChkDept = "Other";
                            library.ChkDeptRef = library.ChkDeptRef;
                        }

                        //library.NoLevel = 0;
                        var getChkLevel     = _context.LevelKaizen_Choose.Where(m => m.NoLevel.Equals(library.NoLevel)).DefaultIfEmpty().FirstOrDefault();
                        if (getChkLevel != null)
                        {
                            library.ChkLevel = getChkLevel.LevelTh + "  (" + getChkLevel.LevelEng + ")";
                        }

                        //library_update.NoType = 0;
                        var getType = _context.TypeKaizen_Choose.Where(m => m.NoType.Equals(library.NoType)).DefaultIfEmpty().FirstOrDefault();
                        if (getType != null)
                        {
                            library.ChkType = getType.TypeTh + "  (" + getType.TypeEng + ")";
                        }

                        //library_update.NoEnv = 0;
                        var getEnv = _context.EnvKaizen_Choose.Where(m => m.NoEnv.Equals(library.NoEnv)).DefaultIfEmpty().FirstOrDefault();
                        if (getEnv != null)
                        {
                              library.ChkEnv = getEnv.EnvTh + "  (" + getEnv.EnvEng + ")";
                        }

                        library.LibraryStatus = "Leader";

                        var library_update = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
                        if (library_update != null)
                        {
                            library.DateEmp = library_update.DateEmp;
                            library.DateLeader = library_update.DateLeader;
                        }

                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;
                        library.DateManager = getDatetime;
                        library.DateSGABoard = getDatetime;

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = view_kaizen.KaizenRef;
                        process.Permission = "Leader";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Checked";

                        //Update Kaizen
                        kaizen_update.KzStatus = "WaitManager";

                        //_context.Update(library_update);

                        //CheckBeforeInsertToDB
                        //if ((library.ChkStandard == "1" && library.ChkStandardRef != "") && (library.ChkDept == "4" && library.ChkDeptRef != ""))
                        //{

                        //}
                         _context.Add(process);
                         _context.Update(kaizen_update);
                         await UpdateLibraryCheck(library, id);
                         await _context.SaveChangesAsync();

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to check!!";
                        return View(library);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(library.LibNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Leader", "Kaizens");
            }
            return View(library);
        }

        //By Leader
        public async Task<IActionResult> UpdateLibraryCheck(Library library, int id)
        {
            var kaizen_update = await _context.Kaizen.FindAsync(id);
            var library_checked = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            if (library_checked != null)
            {
                //Update Library
                library_checked.KaizenRef                   = library.KaizenRef;
                library_checked.ChkActType                  = library.ChkActType;
                library_checked.DateEmp                     = library.DateEmp;
                library_checked.IdEmp                       = library.IdEmp;
                library_checked.DeptEmp                     = library.DeptEmp;
                library_checked.JbtEmp                      = library.JbtEmp;
                library_checked.DateLeader                  = library.DateLeader;
                library_checked.IdLeader                    = library.IdLeader;
                library_checked.NameLeader                  = library.NameLeader;
                library_checked.DeptLeader                  = library.DeptLeader;
                library_checked.JbtLeader                   = library.JbtLeader;
                library_checked.ChkImproved                 = library.ChkImproved;
                library_checked.ChkStandard                 = library.ChkStandard;
                library_checked.ChkStandardRef              = library.ChkStandardRef;
                library_checked.ChkDept                     = library.ChkDept;
                library_checked.ChkDeptRef                  = library.ChkDeptRef;
                library_checked.NoLevel                     = library.NoLevel;
                library_checked.ChkLevel                    = library.ChkLevel;
                library_checked.NoType                      = library.NoType;
                library_checked.ChkType                     = library.ChkType;
                library_checked.ChkCost                     = library.ChkCost;
                library_checked.NoEnv                       = library.NoEnv;
                library_checked.ChkEnv                      = library.ChkEnv;
                library_checked.RemarkLeader                = library.RemarkLeader;

                library_checked.DateManager                 = library.DateManager;
                library_checked.IdManager                   = library.IdManager;
                library_checked.NameManager                 = library.NameManager;
                library_checked.DeptManager                 = library.DeptManager;
                library_checked.JbtManager                  = library.JbtManager;
                library_checked.CntKaizen                   = library.CntKaizen;
                library_checked.CntCR                       = library.CntCR;
                library_checked.CntVote                     = library.CntVote;
                library_checked.RemarkManager               = library.RemarkManager;

                library_checked.DateSGABoard                = library.DateSGABoard;
                library_checked.IdSGABoard                  = library.IdSGABoard;
                library_checked.NameSGABoard                = library.NameSGABoard;
                library_checked.DeptSGABoard                = library.DeptSGABoard;
                library_checked.JbtSGABoard                 = library.JbtSGABoard;
                library_checked.Award                       = library.Award;
                library_checked.Price                       = library.Price;
                library_checked.RemarkSGABoard              = library.RemarkSGABoard;
                library_checked.LibraryStatus               = library.LibraryStatus;
            }

            _context.Update(library_checked);
            return View(library_checked);
        }


        // GET: Kaizens/Check/5  For Manager Only
        public async Task<IActionResult> Approval(int? id, int? searchNo)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionManager = HttpContext.Session.GetString(SessionEmpManager);
  
            if (strSessionManager == "Manager")
            {
                var varDueDate = _context.DueDate.Select(s => new SelectListItem
                {
                    Value = s.DueDateNo.ToString(),
                    Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
                }).OrderByDescending(r => r.Value).ToList();

                ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
                ViewBag.Search = "--- แสดงผลทั้งหมด ---";
                ViewBag.CntKaizen = "-";
                ViewBag.SumPrice = "-";
                ViewBag.SumPrice = "-";

                var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitManager")).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
                //if (view_kaizen_index != null)
                //{
                //    view_kaizen_index = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("Vote")).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
                //}
                if (searchNo != null)
                {
                    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                    DateTime StartingDate = getDueDate.StartingDate;
                    DateTime EndingDate = getDueDate.EndingDate;

                    view_kaizen_index = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitManager") && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                    //Show Search
                    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                    //Cnt
                    var cntKaizen = view_kaizen_index.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitManager") && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;

                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("WaitManager") && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }

                // Check if the list is empty before passing to view
                if (view_kaizen_index == null || !view_kaizen_index.Any())
                {
                    view_kaizen_index = new List<View_Kaizens>();
                }

                if (view_kaizen_index[0] == null)
                {
                    view_kaizen_index = new List<View_Kaizens>();
                }

                return View("Manager", view_kaizen_index);

            }
            else
            {
                return View("Index");
            }
 
    
        }

        //By Manager
        public async Task<IActionResult> ApprovalClick(int KaizenNo, int CntKaizen, int CntCR, int CntVote, Library library, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (strSessionEmpNumRow != null)
            {
                try
                {
                   // var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_update.KzStatus == "WaitManager")
                    {
                        if (CntKaizen == 1)
                        {
                            library.CntKaizen = "Yes";
                            //========Fix Cost beginning=======
                            //Get Award and Price
                            var getAward = _context.Awards.Where(m => m.AwardNo.Equals(Convert.ToInt16(1))).DefaultIfEmpty().FirstOrDefault();
                            library.AwardNo = getAward.AwardNo;
                            library.Award = getAward.Detail;
                            library.Price = getAward.Price;

                            //library.Award = "การมีส่วนร่วม (Participation)";
                            //library.Price = 20;
                            //========End Cost beginning=======
                        }
                        else if (CntKaizen == 2)
                        {
                            library.CntKaizen = "No";
                            //========Fix Cost Not Kaizen=======
                            //Get Award and Price
                            var getAward = _context.Awards.Where(m => m.AwardNo.Equals(Convert.ToInt16(18))).DefaultIfEmpty().FirstOrDefault();
                            library.AwardNo = getAward.AwardNo;
                            library.Award = getAward.Detail;
                            library.Price = getAward.Price;
                            //========End Cost beginning=======
                        }

                        if (CntCR == 1)
                        {
                            library.CntCR = "New";
                        }
                        else if (CntCR == 2)
                        {
                            library.CntCR = "Currently";
                        }
                        else if (CntCR == 3)
                        {
                            library.CntCR = "N/A";
                        }

                        if (CntVote == 1)
                        {
                            library.CntVote = "Yes";
                            //Update Kaizen
                            kaizen_update.KzStatus = "Vote"; //Send to Vote
                        }
                        else if (CntVote == 2)
                        {
                            library.CntVote = "No";
                            //Update Kaizen
                            kaizen_update.KzStatus = "Completed";
                        }

                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;

                        //UpdateDateKaizen
                        kaizen_update.Date = getDatetime;

                        library.LibraryStatus = "Manager";
                        //Get Datetime
                        //DateTime getDatetime = DateTime.Now;
                        library.DateManager = getDatetime;

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen_update.KaizenRef;
                        process.Permission = "Manager";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Approved";

                        _context.Add(process);
                        _context.Update(kaizen_update);
                        await UpdateLibraryApprove(library, KaizenNo);

                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to approval!!";
                        return View(library);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(library.LibNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Approval", "Kaizens");
                //return View("Manager");
            }

            //return View("Manager");
            return RedirectToAction("Approval", "Kaizens");
        }

        //By Manager
        public async Task<IActionResult> UpdateLibraryApprove(Library library, int KaizenNo)
            {
            //var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
			var library_checked = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            if (library_checked != null)
            {
                //Update Library
                library_checked.DateManager = library.DateManager;
                library_checked.CntKaizen = library.CntKaizen;
                library_checked.CntCR = library.CntCR;
                library_checked.CntVote = library.CntVote;
                library_checked.AwardNo = library.AwardNo;
                library_checked.Award = library.Award;
                library_checked.Price = library.Price;
                library_checked.LibraryStatus = library.LibraryStatus;
            }

            _context.Update(library_checked);
            return View("Manager");
        }

        //By Manager
        public async Task<IActionResult> Vote(int? id, int? searchNo)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionManager = HttpContext.Session.GetString(SessionEmpManager);

            if (strSessionManager == "Manager")
            {
                var varDueDate = _context.DueDate.Select(s => new SelectListItem
                {
                    Value = s.DueDateNo.ToString(),
                    Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
                }).OrderByDescending(r => r.Value).ToList();

                ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
                ViewBag.Search = "--- แสดงผลทั้งหมด ---";
                ViewBag.CntKaizen = "-";
                ViewBag.SumPrice = "-";
                ViewBag.SumPrice = "-";

                var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("Vote")).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
                if (searchNo != null)
                {
                    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                    DateTime StartingDate = getDueDate.StartingDate;
                    DateTime EndingDate = getDueDate.EndingDate;

                    view_kaizen_index = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("Vote") && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                    //Show Search
                    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                    //Cnt
                    var cntKaizen = view_kaizen_index.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("Vote") && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;

                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => m.IdManager.Equals(Convert.ToInt32(strSessionEmpId)) && m.KzStatus.Equals("Vote") && m.Date >= StartingDate && m.Date <= EndingDate).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }
                return View("Vote", view_kaizen_index);
            }
            else
            {
                return RedirectToAction("Vote", "Kaizens");
            }

        }

        //By Manager
        public async Task<IActionResult> VoteCancel(int KaizenNo, Library library, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_update.KzStatus == "Vote")
                    {
                        kaizen_update.KzStatus = "Completed";
         
                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;
                        library.DateManager = getDatetime;
                        library.CntVote = "No"; //Note Vote

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();

                        //Insert Process
                        process.KaizenRef = kaizen_update.KaizenRef;
                        process.Permission = "Manager";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "CancelVote";

                        _context.Add(process);
                        _context.Update(kaizen_update);
                        await UpdateNoteVote(library, KaizenNo);

                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to note vote!!";
                        return View(library);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(library.LibNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Vote", "Kaizens");
            }

            return RedirectToAction("Vote", "Kaizens");
        }

        //By Manager
        public async Task<IActionResult> UpdateNoteVote(Library library, int KaizenNo)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
			//var library_checked = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).FirstOrDefault();
			var library_checked = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
			if (library_checked != null)
            {
                //Update Library
                library_checked.DateManager = library.DateManager;
                library_checked.CntVote = library.CntVote;
            }

            _context.Update(library_checked);
            return RedirectToAction("Vote", "Kaizens");
        }

        //By SGA Board
        public async Task<IActionResult> VoteAward(int? id, int? searchNo, string searchDept)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }
        
            string strSessionAdmin = HttpContext.Session.GetString(SessionEmpAdmin);

            if (searchNo == 0)
            {
                searchNo = null;
            }

            if (searchDept == "--- แสดงผลทั้งหมด ---")
            {
                searchDept = null;
            }

            if (strSessionAdmin == "Admin")
            {
                var varDueDate = _context.DueDate.Select(s => new SelectListItem
                {
                    Value = s.DueDateNo.ToString(),
                    Text = "จากวันที่ " + s.StartingDate + " ถึงวันที่ " + s.EndingDate
                }).OrderByDescending(r => r.Value).ToList();

                ViewBag.DueDate = new SelectList(varDueDate, "Value", "Text");
                ViewBag.Search = "--- แสดงผลทั้งหมด ---";
                ViewBag.CntKaizen = "-";
                ViewBag.SumPrice = "-";
                ViewBag.SumPrice = "-";

                var varDeptTab = _context.View_GrpDeptTab.Select(s => new SelectListItem
                {
                    Value = s.DEPT_CODE.ToString(),
                    Text = s.DEPT_CODE + ": " + s.DEPT_ENGDESC + " " + s.DEPT_THAIDESC
                }).OrderBy(r => r.Value).ToList();

                ViewBag.DeptTab = new SelectList(varDeptTab, "Value", "Text");

				var varChkType = _context.ActType_Choose.Select(s => new SelectListItem
				{
					Value = s.NoActType.ToString(),
					Text = s.ActTypeTh + " (" + s.ActTypeEng + ")"
				}).ToList();

				ViewBag.ChkType = new SelectList(varChkType, "Value", "Text");

				var varAward = _context.Awards.Select(s => new SelectListItem
                {
                    Value = s.AwardNo.ToString(),
                    Text = s.Detail + " " + s.Price
                }).ToList();

                ViewBag.Award = new SelectList(varAward, "Value", "Text");

                //1 searchNo == null && searchDept == null
                var view_kaizen_vote = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award" ) && (m.LibraryStatus != "SGABoard") ).OrderByDescending(i => i.DateManager).DefaultIfEmpty().ToList();

                //2 searchNo != null && searchDept == null
                if (searchNo != null && searchDept == null)
                {
                    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                    DateTime StartingDate = getDueDate.StartingDate;
                    DateTime EndingDate = getDueDate.EndingDate;

                    view_kaizen_vote = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.LibraryStatus != "SGABoard") && (m.Date >= StartingDate && m.Date <= EndingDate)).OrderByDescending(i => i.DateManager).DefaultIfEmpty().ToList();

                    //Show Search
                    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                    //Cnt
                    var cntKaizen = view_kaizen_vote.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;

                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }

                //3 searchNo == null && searchDept != null
                if (searchNo == null && searchDept != null)
                {
                    var getDeptNo = _context.View_GrpDeptTab.Where(d => d.DEPT_CODE == searchDept).DefaultIfEmpty().FirstOrDefault();
                    String DEPT_TH = getDeptNo.DEPT_THAIDESC;
                    String Dept_ENG = getDeptNo.DEPT_ENGDESC;
                    string sDeptNo = searchDept.Substring(0, 4);

                    view_kaizen_vote = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.LibraryStatus != "SGABoard") && m.DeptNo.StartsWith(sDeptNo)).OrderByDescending(i => i.DateManager).DefaultIfEmpty().ToList();

                    //Show Search
                    ViewBag.Search = "แผนก " + Dept_ENG + " " + DEPT_TH;

                    //Cnt
                    var cntKaizen = view_kaizen_vote.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && m.DeptNo.StartsWith(sDeptNo)).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;

                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && m.DeptNo.StartsWith(sDeptNo)).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }
                //4 searchNo != null && searchDept != null
                if (searchNo != null && searchDept != null)
                {
                    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).DefaultIfEmpty().FirstOrDefault();
                    DateTime StartingDate = getDueDate.StartingDate;
                    DateTime EndingDate = getDueDate.EndingDate;

                    var getDeptNo = _context.View_GrpDeptTab.Where(d => d.DEPT_CODE == searchDept).DefaultIfEmpty().FirstOrDefault();
                    String DEPT_TH = getDeptNo.DEPT_THAIDESC;
                    String Dept_ENG = getDeptNo.DEPT_ENGDESC;
                    string sDeptNo = searchDept.Substring(0, 4);

                    view_kaizen_vote = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.LibraryStatus != "SGABoard") && (m.Date >= StartingDate && m.Date <= EndingDate)).OrderByDescending(i => i.DateManager).DefaultIfEmpty().ToList();

                    //Show Search
                    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy") + " ของแผนก " + Dept_ENG + " " + DEPT_TH;

                    //Cnt
                    var cntKaizen = view_kaizen_vote.Count();
                    ViewBag.CntKaizen = cntKaizen;

                    //Sum Price
                    var sumPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.DeptNo.StartsWith(sDeptNo)) && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.Price);
                    string FormattedsumPrice = sumPrice.ToString("N");
                    ViewBag.SumPrice = FormattedsumPrice;

                    //Sum CR Price
                    var sumCRPrice = _context.View_Kaizens.Where(m => (m.KzStatus == "Vote" || m.KzStatus == "Award") && (m.DeptNo.StartsWith(sDeptNo)) && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.ChkCost);
                    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                    ViewBag.SumCRPrice = FormattedsumCRPrice;
                }

                searchNo = null;
                searchDept = null;

                return View("VoteAward", view_kaizen_vote);

                //var view_kaizen_index = _context.View_Kaizens.Where(m => m.KzStatus.Equals("Vote") || m.KzStatus.Equals("Award")).OrderByDescending(i => i.KaizenNo).ToList();
                //if (searchNo != null)
                //{
                //    var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).FirstOrDefault();
                //    DateTime StartingDate = getDueDate.StartingDate;
                //    DateTime EndingDate = getDueDate.EndingDate;

                //    view_kaizen_index = _context.View_Kaizens.Where(m => m.KzStatus.Equals("Vote") || m.KzStatus.Equals("Award") && (m.Date >= StartingDate && m.Date <= EndingDate)).OrderByDescending(i => i.KaizenNo).ToList();

                //    //Show Search
                //    ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");

                //    //Cnt
                //    var cntKaizen = view_kaizen_index.Count();
                //    ViewBag.CntKaizen = cntKaizen;

                //    //Sum Price
                //    var sumPrice = _context.View_Kaizens.Where(m => m.KzStatus.Equals("Vote") || m.KzStatus.Equals("Award") && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.Price);
                //    string FormattedsumPrice = sumPrice.ToString("N");
                //    ViewBag.SumPrice = FormattedsumPrice;

                //    //Sum CR Price
                //    var sumCRPrice = _context.View_Kaizens.Where(m => m.KzStatus.Equals("Vote") || m.KzStatus.Equals("Award") && (m.Date >= StartingDate && m.Date <= EndingDate)).Sum(s => s.ChkCost);
                //    string FormattedsumCRPrice = sumCRPrice.ToString("N");
                //    ViewBag.SumCRPrice = FormattedsumCRPrice;
                //}
              
            }
            else
            {
                return View("Index");
                
            }

            //return View("VoteAward", view_kaizen);

        }

        public async Task<IActionResult> VoteAwardClick(int KaizenNo, int NoType, int AwardNo, Library library, Process process)
        {
            Session();
            string strSessionEmpId = HttpContext.Session.GetString(SessionEmpId);
            if (strSessionEmpId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string strSessionEmpNumRow = HttpContext.Session.GetString(SessionEmpNumRow);
            if (strSessionEmpNumRow != null)
            {
                try
                {
                    //var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
					var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
					if (kaizen_update == null)
                    {
                        return NotFound();
                    }

                    if (kaizen_update.KzStatus == "Vote" || kaizen_update.KzStatus == "Award")
                    {

                        //Get Datetime
                        DateTime getDatetime = DateTime.Now;
                        library.DateSGABoard = getDatetime;

						//Get NoActType
						var getNoActType = _context.ActType_Choose.Where(m => m.NoActType.Equals(Convert.ToInt16(NoType))).DefaultIfEmpty().FirstOrDefault();
						library.NoType = getNoActType.NoActType;
						library.ChkType = getNoActType.ActTypeTh + " (" + getNoActType.ActTypeEng + ")";

						//Get Award and Price
						var getAward = _context.Awards.Where(m => m.AwardNo.Equals(Convert.ToInt16(AwardNo))).DefaultIfEmpty().FirstOrDefault();
                        library.AwardNo = getAward.AwardNo;
                        library.Award = getAward.Detail;
                        library.Price = getAward.Price;

                        kaizen_update.KzStatus = "Award"; //Save to history of award
                        library.LibraryStatus = "SGABoard";

                        //Get Dept. Emp
                        var getDeptNo = _context.View_ApprovalFlow.Where(m => m.Num_Row.Equals(Convert.ToInt16(strSessionEmpNumRow))).DefaultIfEmpty().FirstOrDefault();
                        //Update SGA Name
                        library.IdSGABoard = getDeptNo.PRS_NO;
                        library.NameSGABoard = getDeptNo.EMP_E_NAME;
                        library.DeptSGABoard = getDeptNo.DEPT_ENGDESC;
                        library.JbtSGABoard = getDeptNo.JBT_ENGDESC;

                        //Insert Process
                        process.KaizenRef = kaizen_update.KaizenRef;
                        process.Permission = "SGABoard";
                        process.Datetime = getDatetime;
                        process.ID = getDeptNo.PRS_NO;
                        process.Status = "Award";

                        _context.Add(process);
                        _context.Update(kaizen_update);
                        await UpdateLibraryAward(library, KaizenNo);

                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        ViewBag.showAlert = true;
                        ViewBag.alertMessage = "*** Status cannot update to award!!";
                        return View(library);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaizenExists(library.LibNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("VoteAward", "Kaizens");
            }

            return View("VoteAward");
        }

        //By Manager
        public async Task<IActionResult> UpdateLibraryAward(Library library, int KaizenNo)
        {
            //var kaizen_update = await _context.Kaizen.FindAsync(KaizenNo);
			var kaizen_update = await _context.Kaizen.DefaultIfEmpty().FirstOrDefaultAsync(e => e.KaizenNo == KaizenNo);
			var library_checked = _context.Library.Where(m => m.KaizenRef.Equals(kaizen_update.KaizenRef)).DefaultIfEmpty().FirstOrDefault();
            if (library_checked != null)
            {
				//Update Library
				library_checked.NoType          = library.NoType;
				library_checked.ChkType         = library.ChkType;
				library_checked.IdSGABoard      = library.IdSGABoard;
                library_checked.NameSGABoard    = library.NameSGABoard;
                library_checked.DeptSGABoard    = library.DeptSGABoard;
                library_checked.JbtSGABoard     = library.JbtSGABoard;
                library_checked.DateSGABoard    = library.DateSGABoard;
                library_checked.AwardNo         = library.AwardNo;
                library_checked.Award           = library.Award;
                library_checked.Price           = library.Price;
                library_checked.LibraryStatus   = library.LibraryStatus;
            }

            _context.Update(library_checked);
            return View("VoteAward");
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var kaizen = await _context.Kaizen.FindAsync(id);
        //    _context.Kaizen.Remove(kaizen);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //// GET: Kaizens/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    Session();
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kaizen = await _context.Kaizen
        //        .FirstOrDefaultAsync(m => m.KaizenNo == id);
        //    if (kaizen == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(kaizen);
        //}

        //// POST: Kaizens/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    Session();
        //    var kaizen = await _context.Kaizen.FindAsync(id);
        //    _context.Kaizen.Remove(kaizen);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool KaizenExists(int id)
        {
            return _context.Kaizen.Any(e => e.KaizenNo == id);
        }
    }

    //public interface IWebHostEnviromemt
    //{
    //    object WebRootPath { get; }
    //}

    internal interface IWebHostEnvironment
    {
		object WebRootPath { get; }
	}

}
