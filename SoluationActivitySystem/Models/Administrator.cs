using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models
{
    public class Administrator
    {
        [Key]
        public int AdminNo { get; set; }
        public int EmpId { get; set; }

    }
}
