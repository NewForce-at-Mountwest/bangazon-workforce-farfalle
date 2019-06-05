using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class TrainingProgram : IValidatableObject
    {

        public int Id { get; set; }
        [Display(Name = "Training Name")]
        public string Name { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        //This ensures that a new training program must have an end date later than the start date
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if  ((DateTime.Compare(EndDate, StartDate) < 0))
            {
                yield return new ValidationResult(
                    $"End date must be later than start date.",
                    new[] { "EndDate" });
            }
        }
    }
}
