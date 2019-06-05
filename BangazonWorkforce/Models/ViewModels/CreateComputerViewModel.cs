using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class CreateComputerViewModel
    {

        public List<SelectListItem> Employees { get; set; }
        public Computer computer {get; set;}
        public int employeeId { get; set; }
        

        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public CreateComputerViewModel() { }

        public CreateComputerViewModel(string connectionString)
        {
            _connectionString = connectionString;

            Employees = GetAllEmployees()
                .Select(employee => new SelectListItem
                {
                    Text = $"{ employee.FirstName } { employee.LastName }",
                    Value = employee.Id.ToString()
                })
                .ToList();

            //Employees.Insert(0, new SelectListItem
            //{
            //    Text = "Assign employee",
            //    Value = "0"
            //});
        }

        private List<Employee> GetAllEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}
