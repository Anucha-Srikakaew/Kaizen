using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models
{
    public class Process
    {
        [Key]
        public int ProcessID { get; set; }
        public string KaizenRef { get; set; }
        public string Permission { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Datetime { get; set; }
        public int ID { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }
}
