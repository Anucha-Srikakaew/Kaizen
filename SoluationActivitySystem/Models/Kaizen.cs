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
    public class Kaizen
    {
        [Key]
        public int KaizenNo { get; set; }

        public string DeptNo { get; set; }

        public string KaizenRef { get; set; }

        //[Required]
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //[Required]
        public int NoActType { get; set; }

        public string? ActDetail { get; set; }

       // [Required(ErrorMessage = "Please choose before image")]
        public string ImpBefore { get; set; }
        [NotMapped]
        public IFormFile? ImageBefore { get; set; }

        //[Required]
        public string Problem { get; set; }

       // [Required(ErrorMessage = "Please choose after image")]
        public string ImpAfter { get; set; }
        [NotMapped]
        public IFormFile? ImageAfter { get; set; }

       // [Required]
        public string Result { get; set; }

        //[Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
        public decimal CostSaving { get; set; }

        public string KzStatus { get; set; }

    }
}
