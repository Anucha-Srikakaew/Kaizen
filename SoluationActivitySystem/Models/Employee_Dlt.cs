using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models
{
    public class Employee
    {
        [Key]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "Emp. ID is Required.")]
        public int EMP_ID { get; set; }

        [Required(ErrorMessage = "Emp. Birth is Required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss }")]
        public DateTime EMP_BIRTH { get; set; }

        public string EMP_E_NAME { get; set; }
        public string DEPT_ENGDESC { get; set; }
        public string JBT_ENGDESC { get; set; }
        public string WBP_EMAIL { get; set; }
        public string Signature { get; set; }
        public string EmpStatus { get; set; }

        public string LeaderStatus { get; set; }
        public string LeaderDept { get; set; }
        public string ManagerStatus { get; set; }
        public string ManagerDept { get; set; }
        public string SGATeamStatus { get; set; }
        public string SGABoaedStatus { get; set; }

       
    }
}