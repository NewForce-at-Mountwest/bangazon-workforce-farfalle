using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    //Used for populating edit form and updating 
    //Authored by Sable Bowen
    public class EditEmployeeViewModel
    {
        public Employee employee { get; set; }
        public List<SelectListItem> departments { get; set; } = new List<SelectListItem>();
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
                    Text =  $"{computer.Make} {computer.Manufacturer}",
                    Value = computer.Id.ToString()
                }).ToList();

            }

            




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
                    cmd.CommandText = @"SELECT Id, Make, Manufacturer FROM Computers JOIN ComputerEmployee ON Computer.Id = ComputerId WHERE EmployeeId = null";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        });
                    }

                    reader.Close();
                    return computers;
                }
            }
        
    }











    }
}
