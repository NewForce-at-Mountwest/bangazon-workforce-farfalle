using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    //Used for populating edit form and updating employees
    //Authored by Sable Bowen
    public class EditEmployeeViewModel
    {
        public Employee employee { get; set; }
        [Display(Name = "Department")]
        public List<SelectListItem> departments { get; set; } = new List<SelectListItem>();
        [Display(Name = "Assigned Computer")]
        public List<SelectListItem> computers { get; set; } = new List<SelectListItem>();

        protected string _connectionString;

        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }



        public EditEmployeeViewModel() { }
        public EditEmployeeViewModel(string connectionString, int id)
        {
            _connectionString = connectionString;

            {

                using (SqlConnection conn = Connection)

                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Employee.Id, FirstName, LastName, DepartmentId, Make, Manufacturer, ComputerEmployee.ComputerId FROM Employee JOIN ComputerEmployee ON Employee.Id = ComputerEmployee.EmployeeId JOIN Computer ON ComputerEmployee.ComputerId = Computer.Id WHERE Employee.Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                CurrentComputer = new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                }

                            };
                            
                        }
                        reader.Close();






                        //Gets all the departments to populate select list
                        departments = GetAllDepartments()
                    .Select(department => new SelectListItem()
                    {
                        Text = department.Name,
                        Value = department.Id.ToString()
                    }).ToList();

                        departments.Insert(0, new SelectListItem
                        {
                            Text = "Choose a department",
                            Value = "0"
                        });


                        //Gets all the computers to populate select list
                        computers = GetAllComputers().Select(computer => new SelectListItem()
                        {
                            Text = $"{computer.Make} {computer.Manufacturer}",
                            Value = computer.Id.ToString()
                        }).ToList();

                    }




                } }

        }
    






        public List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }

                    reader.Close();
                    return departments;
                }
            }
        }

        public List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT ComputerId, Computer.Id, Make, Manufacturer, UnassignDate, EmployeeId FROM Computer FULL JOIN ComputerEmployee ON Computer.Id = ComputerId";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        //Checks to see if computer is not already in the list and unassigned before adding it to the list
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")) && !reader.IsDBNull(reader.GetOrdinal("UnassignDate")))
                        {
                            if (!computers.Any(x => x.Id == reader.GetInt32(reader.GetOrdinal("Id")))){
                                computers.Add(new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                });

                            }

                            //Checks to see if computer has ever been assigned, or if it is in the list before adding it
                            if (reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                            {
                                if (!computers.Any(x => x.Id == reader.GetInt32(reader.GetOrdinal("Id"))))
                                {
                                    computers.Add(new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                    });
                                }
                            }
                        }


                       

                    }

                    reader.Close();
                    return computers;
                }
            }
        
    }











    }
}
