using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivitySystem.Models
{
    public class ActType_Choose
    {

        [Key]
        public int NoActType { get; set; }
        public string ActTypeTh { get; set; }
        public string ActTypeEng { get; set; }

    }
}
