using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models
{
    public class LevelKaizen_Choose
    {
        [Key]
        public int NoLevel { get; set; }
        public string LevelTh { get; set; }
        public string LevelEng { get; set; }
    }
}
