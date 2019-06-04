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
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();

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
