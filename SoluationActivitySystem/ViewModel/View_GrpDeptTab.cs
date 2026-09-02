using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ActivitySystem.ViewModel
{
    public class View_GrpDeptTab
    {
        [Key]
        public string DEPT_CODE { get; set; }
        public string DEPT_THAIDESC { get; set; }
        public string DEPT_ENGDESC { get; set; }
    }
}
