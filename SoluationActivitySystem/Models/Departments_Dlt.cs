using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models
{
    public class Departments
    {
        [Key]
        public int DeptNo { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string RunningCode { get; set; }
        public int RunningNumber { get; set; }
        public int TargetMonth { get; set; }
    }
}
