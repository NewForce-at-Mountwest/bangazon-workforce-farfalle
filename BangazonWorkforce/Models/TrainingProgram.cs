using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class TrainingProgram
    {
        [Display(Name = "Program Id")]
        public int Id { get; set; }
        [Display(Name = "Program Name")]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();


    }
}
