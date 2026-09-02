using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivitySystem.Models
{
    public class Awards
    {
        [Key]
        public int AwardNo { get; set; }
        public string Detail { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
        public Decimal Price { get; set; }

    }
}
