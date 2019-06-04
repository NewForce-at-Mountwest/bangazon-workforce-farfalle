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
        [Display(Name = "Purchase Date")]

        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Decommission Date")]

        public DateTime? DecomissionDate { get; set; }
        public string Make { get; set; }
        public string Manufacturer { get; set; }
    }
}
