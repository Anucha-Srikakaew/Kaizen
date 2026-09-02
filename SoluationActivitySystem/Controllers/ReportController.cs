using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ActivitySystem.Data;
using ActivitySystem.ViewModel;
using Microsoft.Extensions.Logging;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Reporting.WebForms;
using Microsoft.Extensions.Configuration;

namespace ActivitySystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly DatabaseContext _context;

        private readonly ILogger<ReportController> _logger;
       // private readonly IWebHostEnviromemt _webHostEnv;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnv;
        private readonly string _connectionString;

        //public View_Kaizen_DataTable view_Kaizen_DataTable = new View_Kaizen_DataTable();

        public ReportController(DatabaseContext context, ILogger<ReportController> logger, Microsoft.AspNetCore.Hosting.IHostingEnvironment HostEnv, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            //_webHostEnv = webHostEnv;
            _HostEnv = HostEnv;
            _connectionString = configuration.GetConnectionString("DataConnection")
                ?? throw new InvalidOperationException("Connection string 'DataConnection' is not configured.");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        const string SessionEmpNumRow   = "_EmpNumRow";
        const string SessionEmpId       = "_EmpId";
        const string SessionEmpName     = "_EmpName";
        const string SessionEmpBirth    = "_EmpBirth";
        const string SessionEmpAdmin    = "_EmpAdmin";
        const string SessionEmpLeader   = "_EmpLeader";
        const string SessionEmpManager  = "_EmpManager";

        //New Session Report
        const string SessionRptStartDate = "_RptStartDate";
        const string SessionRptEndDate   = "_RptEndDate";
        const string SessionSearchNo     = "_SearchNo";

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
                ViewBag.RptStartDate = HttpContext.Session.GetString(SessionRptStartDate);
                ViewBag.RptEndDate = HttpContext.Session.GetString(SessionRptEndDate);
                ViewBag.SearchNo = HttpContext.Session.GetString(SessionSearchNo);
                return RedirectToAction("UserProfile", "Login");
            }
            else
            {
                //return View("Login", "Login");
                return RedirectToAction("Login", "Login");
            }

        }

        public IActionResult Report(int? searchNo)
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
            ViewBag.Search = "--- กรุณาเลือกวันที่รอบกิจกรรมก่อนทำการเลือกประเภทรายงาน ---";

            var view_kaizen_index = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId))).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();
            if (searchNo != null)
            {
                var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(searchNo))).FirstOrDefault();
                DateTime StartingDate = getDueDate.StartingDate;
                DateTime EndingDate = getDueDate.EndingDate;

                view_kaizen_index = _context.View_Kaizens.Where(m => m.IdEmp.Equals(Convert.ToInt32(strSessionEmpId)) && m.Date >= StartingDate && m.Date <= EndingDate).OrderByDescending(i => i.KaizenNo).DefaultIfEmpty().ToList();

                //Show Search
                ViewBag.Search = "จากวันที่ " + StartingDate.ToString("dd/MM/yyyy") + " ถึงวันที่ " + EndingDate.ToString("dd/MM/yyyy");
                //+ " : " + searchNo.ToString(); ;

                //Add New Session
                HttpContext.Session.SetString(SessionRptStartDate, StartingDate.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(SessionRptEndDate, EndingDate.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(SessionSearchNo, searchNo.ToString());

                ViewBag.RptStartDate = StartingDate.ToString("yyyy-MM-dd");
                ViewBag.RptEndDate   = EndingDate.ToString("yyyy-MM-dd");
                ViewBag.SearchNo    = searchNo.ToString();
            }

            return View(view_kaizen_index);
        }

        public ActionResult rptVoteKaizen()
        {
            Session();
            string strSessionSearchNo = HttpContext.Session.GetString(SessionSearchNo);
            if (strSessionSearchNo == null)
            {
                return RedirectToAction("Report", "Report");
            }

            var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(strSessionSearchNo))).DefaultIfEmpty().FirstOrDefault();
            DateTime StartingDate = getDueDate.StartingDate;
            DateTime EndingDate = getDueDate.EndingDate;

            //var dt = new DataTable();
            //dt = view_Kaizen_DataTable.GetView_Kaizen();
            //Get Data StoredProcedure
            string constr = _connectionString;

            var dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {

                SqlCommand cmd = new SqlCommand("SP_Kaizen_Query", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@prStartDate";
                param1.Value = StartingDate.ToString("yyyy-MM-dd");
                //param1.Value = "2021-08-01";
                cmd.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@prEndDate";
                param2.Value = EndingDate.ToString("yyyy-MM-dd");
                //param2.Value = "2021-08-31";
                cmd.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@prTypeReport";
                param3.Value = "1";
                //param2.Value = "2021-08-31";
                cmd.Parameters.Add(param3);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            //END Get Data StoredProcedure

            string mimetype = "";
            int extension = 1;
            //string path_Root = _HostEnv.WebRootPath;
            //var path = $"{_HostEnv.WebRootPath}\\Reports\\rptVoteKaizen.rdlc";
            //var path = "D:/Application/SolutionKaizenProject/wwwroot/Reports/rptVoteKaizen.rdlc";
            //var path =  Path.Combine("D:Application\\SolutionKaizenProject\\wwwroot\\Reports", "rptVoteKaizen.rdlc");

            string path_Root = _HostEnv.WebRootPath;
            var path = path_Root + "\\Reports\\rptVoteKaizen.rdlc";

           // var path = Path.Combine(_HostEnv.WebRootPath, "Reports", "rptSumKaizen.rdlc");


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("prm1", "Vote Kaizen Report");
            parameters.Add("prm2", StartingDate.ToString("dd-MM-yyyy"));
            parameters.Add("prm3", EndingDate.ToString("dd-MM-yyyy"));

            //parameters.Add("prm1", "RDLC Report");
            //parameters.Add("prm2", DateTime.Now.ToString("dd-MM-yyyy"));
            //parameters.Add("prm3", "Vote Kaizen Report");

            //ReportParameter[] parameters = new ReportParameter[];
            //parameters[0] = new ReportParameter("myparam1", "my value1");
            //parameters[1] = new ReportParameter("myparam2", "my value2");
            //parameters[2] = new ReportParameter("myparam2", "my value2");
            //this.reportViewer1.LocalReport.SetParameters(parms);
            //this.reportViewer1.RefreshReport();


            AspNetCore.Reporting.LocalReport localReport = new AspNetCore.Reporting.LocalReport(path);
            localReport.AddDataSource("DataSet_View_Kaizen", dt);
            //D:\\000PROJECT2024000\\KaizenFY24\\SoluationActivitySystem\\SoluationActivitySystem\\wwwroot\\Reports\\rptVoteKaizen.rdlc

            //var res = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
            //return File(res.MainStream, "application/pdf");

            try
            {
                var res = localReport.Execute(RenderType.Excel, extension, parameters, mimetype);
                return File(res.MainStream, "application/msexcel", "VoteKaizenExport" + StartingDate.ToString("yyyy-MM-dd") + "To" + EndingDate.ToString("yyyy-MM-dd") + ".xls");
            }
            catch (LocalProcessingException ex)
            {
                // Log the error and provide more details.
                Console.WriteLine(ex.Message);
                throw;
            }
        }




        public ActionResult rptSumKaizen()
        {
            Session();
            string strSessionSearchNo = HttpContext.Session.GetString(SessionSearchNo);
            if (strSessionSearchNo == null)
            {
                return RedirectToAction("Report", "Report");
            }

            var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(strSessionSearchNo))).DefaultIfEmpty().FirstOrDefault();
            DateTime StartingDate = getDueDate.StartingDate;
            DateTime EndingDate = getDueDate.EndingDate;

            string constr2 = _connectionString;

            var dt2 = new DataTable();
            using (SqlConnection con2 = new SqlConnection(constr2))
            {

                SqlCommand cmd2 = new SqlCommand("SP_Kaizen_Query", con2);
                cmd2.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@prStartDate";
                param1.Value = StartingDate.ToString("yyyy-MM-dd");
                cmd2.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@prEndDate";
                param2.Value = EndingDate.ToString("yyyy-MM-dd");
                cmd2.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@prTypeReport";
                param3.Value = "2"; //Sum Kaizen
                cmd2.Parameters.Add(param3);

                con2.Open();
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                da2.Fill(dt2);
                con2.Close();
            }
            //END Get Data StoredProcedure

            string mimetype2 = "";
            int extension2 = 1;

            string path_Root2 = _HostEnv.WebRootPath;
            var path2 = path_Root2 + "\\Reports\\rptSumKaizen.rdlc";
            //var path2 = path_Root2 + "\\Reports\\rptVoteKaizen.rdlc";


            //var path2 = $"{_HostEnv.WebRootPath}\\Reports\\rptSumKaizen.rdlc";
            //var path2 = "D:/Application/SolutionKaizenProject/wwwroot/Reports/rptSumKaizen.rdlc";
            //var path2 = Path.Combine("D:Application\\SolutionKaizenProject\\wwwroot\\Reports", "rptSumKaizen.rdlc");

            Dictionary<string, string> parameters2 = new Dictionary<string, string>();
            parameters2.Add("prm1", "Summary Kaizen Report");
            parameters2.Add("prm2", StartingDate.ToString("dd-MM-yyyy"));
            parameters2.Add("prm3", EndingDate.ToString("dd-MM-yyyy"));

            AspNetCore.Reporting.LocalReport localReport2 = new AspNetCore.Reporting.LocalReport(path2);
            localReport2.AddDataSource("DataSet_View_Kaizen", dt2);

            //var res = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
            //return File(res.MainStream, "application/pdf");

            try
            {
                var res2 = localReport2.Execute(RenderType.Excel, extension2, parameters2, mimetype2);
                return File(res2.MainStream, "application/msexcel", "SummaryKaizenExport" + StartingDate.ToString("yyyy-MM-dd") + "To" + EndingDate.ToString("yyyy-MM-dd") + ".xls");
            }
            catch (LocalProcessingException ex)
            {
                // Log the error and provide more details.
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public ActionResult rptSignKaizen()
        {
            Session();
            string strSessionSearchNo = HttpContext.Session.GetString(SessionSearchNo);
            if (strSessionSearchNo == null)
            {
                return RedirectToAction("Report", "Report");
            }

            var getDueDate = _context.DueDate.Where(m => m.DueDateNo.Equals(Convert.ToInt16(strSessionSearchNo))).DefaultIfEmpty().FirstOrDefault();
            DateTime StartingDate = getDueDate.StartingDate;
            DateTime EndingDate = getDueDate.EndingDate;

            string constr3 = _connectionString;

            var dt3 = new DataTable();
            using (SqlConnection con3 = new SqlConnection(constr3))
            {

                SqlCommand cmd3 = new SqlCommand("SP_Kaizen_Query", con3);
                cmd3.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@prStartDate";
                param1.Value = StartingDate.ToString("yyyy-MM-dd");
                cmd3.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@prEndDate";
                param2.Value = EndingDate.ToString("yyyy-MM-dd");
                cmd3.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@prTypeReport";
                param3.Value = "2"; //Sum Kaizen
                cmd3.Parameters.Add(param3);

                con3.Open();
                SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                da3.Fill(dt3);
                con3.Close();
            }
            //END Get Data StoredProcedure

            string mimetype3 = "";
            int extension3 = 1;
            string path_Root3 = _HostEnv.WebRootPath;
            var path3 = path_Root3 + "\\Reports\\rptSignatureKaizen.rdlc";

            //var path3 = "D:/Application/SolutionKaizenProject/wwwroot/Reports/rptSignatureKaizen.rdlc";
            //var path3 = Path.Combine("D:Application\\SolutionKaizenProject\\wwwroot\\Reports", "rptSignatureKaizen.rdlc");

            Dictionary<string, string> parameters3 = new Dictionary<string, string>();
            parameters3.Add("prm1", "Signature Kaizen Report");
            parameters3.Add("prm2", StartingDate.ToString("dd-MM-yyyy"));
            parameters3.Add("prm3", EndingDate.ToString("dd-MM-yyyy"));

            AspNetCore.Reporting.LocalReport localReport3 = new AspNetCore.Reporting.LocalReport(path3);
            localReport3.AddDataSource("DataSet_View_Kaizen", dt3);

            //var res = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
            //return File(res.MainStream, "application/pdf");
            try
            {
                var res3 = localReport3.Execute(RenderType.Excel, extension3, parameters3, mimetype3);
                return File(res3.MainStream, "application/msexcel", "SignatureKaizenExport" + StartingDate.ToString("yyyy-MM-dd") + "To" + EndingDate.ToString("yyyy-MM-dd") + ".xls");
            }
            catch (LocalProcessingException ex)
            {
                // Log the error and provide more details.
                Console.WriteLine(ex.Message);
                throw;
            }

        }
    }
}
