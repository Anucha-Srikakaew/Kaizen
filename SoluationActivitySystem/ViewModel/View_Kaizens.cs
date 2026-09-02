using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ActivitySystem.ViewModel
{
    public class View_Kaizens
    {
        //Table Kaizen  
        [Key]
        public int KaizenNo { get; set; }
        public string DeptNo { get; set; }
        public string KaizenRef { get; set; }
        public string Title { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int NoActType { get; set; }
        public string ActDetail { get; set; }
        //[Required(ErrorMessage = "Please choose before image")]
        public string ImpBefore { get; set; }
        public string Problem { get; set; }
        //[Required(ErrorMessage = "Please choose after image")]
        public string ImpAfter { get; set; }
        public string Result { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal CostSaving { get; set; }
        public string KzStatus { get; set; }

        //Table ActType_Choose
        public string ActTypeTh { get; set; }
        public string ActTypeEng { get; set; }

        //Table Library
        public int LibNo { get; set; }
        public string ChkActType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateEmp { get; set; }
        public int IdEmp { get; set; }
        public string NameEmp { get; set; }
        public string DeptEmp { get; set; }
        public string JbtEmp { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLeader { get; set; }
        public int IdLeader { get; set; }
        public string NameLeader { get; set; }
        public string DeptLeader { get; set; }
        public string JbtLeader { get; set; }
        public string ChkImproved { get; set; }
        public string ChkStandard { get; set; }
        public string ChkStandardRef { get; set; }
        public string ChkDept { get; set; }
        public string ChkDeptRef { get; set; }
        public int NoLevel { get; set; }
        public string ChkLevel { get; set; }
        public int NoType { get; set; }
        public string ChkType { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Decimal ChkCost { get; set; }
        public int NoEnv { get; set; }
        public string ChkEnv { get; set; }
        public string RemarkLeader { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateManager { get; set; }
        public int IdManager { get; set; }
        public string NameManager { get; set; }
        public string DeptManager { get; set; }
        public string JbtManager { get; set; }
        public string CntKaizen { get; set; }
        public string CntCR { get; set; }
        public string CntVote { get; set; }
        public string RemarkManager { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSGABoard { get; set; }
        public int IdSGABoard { get; set; }
        public string NameSGABoard { get; set; }
        public string DeptSGABoard { get; set; }
        public string JbtSGABoard { get; set; }
        public int AwardNo { get; set; }
        public string Award { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Decimal Price { get; set; }
        public string RemarkSGABoard { get; set; }
        public string LibraryStatus { get; set; }


    }
}
