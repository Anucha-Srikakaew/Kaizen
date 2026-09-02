using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ActivitySystem.ViewModel
{
    public class View_Manager
    {
        [Key]
        public int Approval_No { get; set; }
        public string Approval_Name { get; set; }
        public string Approval_DeptCode { get; set; }
        public string Approval_Jbt { get; set; }
    }
}
