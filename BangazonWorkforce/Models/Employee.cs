﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models 
{
    public class Employee : IValidatableObject
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsSuperVisor { get; set; }
        [Display(Name = "Department")]
        public Department CurrentDepartment { get; set; }

        public Computer CurrentComputer { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DepartmentId == 0)
            {
                yield return new ValidationResult(
                    $"Employees must be assigned to a department.",
                    new[] { "DepartmentId" });
            }
        }

    }
}
