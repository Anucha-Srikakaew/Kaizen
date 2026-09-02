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
    public class EnvKaizen_Choose
    {
        [Key]
        public int NoEnv { get; set; }
        public string EnvTh { get; set; }
        public string EnvEng { get; set; }
    }
}
