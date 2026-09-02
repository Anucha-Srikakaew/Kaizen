using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivitySystem.Models
{
    public class TypeKaizen_Choose
    {
        [Key]
        public int NoType { get; set; }
        public string TypeTh { get; set; }
        public string TypeEng { get; set; }
    }
}
