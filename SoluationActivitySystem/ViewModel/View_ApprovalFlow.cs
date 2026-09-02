using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.ViewModel
{
    public class View_ApprovalFlow
    {
        [Key]
        public int Num_Row { get; set; }

        [Required(ErrorMessage = "Emp. ID is Required.")]
        public int PRS_NO { get; set; }

        public string EMP_INTL { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_SURNME { get; set; }
        public string EMP_E_NAME { get; set; }

        [Required(ErrorMessage = "Emp. Birth is Required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss }")]
        public DateTime EMP_BIRTH { get; set; }

        public DateTime PRS_SC_D { get; set; }
        public string EMP_EMAIL { get; set; }
        public string JBT_CODE { get; set; }
        public string JBT_ENGDESC { get; set; }
        public string JBT_THAIDESC { get; set; }
        public int    PRS_DEPT { get; set; }
        public string DEPT_CODE { get; set; }
        public string DEPT_ENGDESC { get; set; }
        public string DEPT_THAIDESC { get; set; }
        public string LAP_CODE { get; set; }
        public int    EDS_APV_TYPE { get; set; }
        public string TypeApp { get; set; }
        public int    EDS_LEVEL { get; set; }
       
        public int Approval_No { get; set; }
        public string Approval_Name { get; set; }
        public string Approval_DeptCode { get; set; }
        public string Approval_Jbt { get; set; }
        public string WBP_NAME { get; set; }
        public string WBP_PASSWORD { get; set; }
        public string WBP_EMAIL { get; set; }
    } 
}
