using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class CreateEmployeeViewModel
    {
        public Employee employee { get; set; }
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();


        protected string _connectionString;

        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public CreateEmployeeViewModel() { }

        public CreateEmployeeViewModel(string connectionString)
        {
            _connectionString = connectionString;

            {
                Departments = GetAllDepartments()
                    .Select(department => new SelectListItem()
                    {
                        Text = department.Name,
                        Value = department.Id.ToString()
                    }).ToList();

                Departments.Insert(0, new SelectListItem
                {
                    Text = "Choose a department",
                    Value = "0"
                });

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
    }
}
