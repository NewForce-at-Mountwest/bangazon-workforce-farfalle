using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; }
        [Display(Name = "Computer Make")]
        public string Make { get; set; }
        [Display(Name = "Computer Manufacturer")]
        public string Manufacturer { get; set; }
    }
}
