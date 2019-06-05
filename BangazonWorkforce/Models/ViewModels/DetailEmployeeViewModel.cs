using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DetailEmployeeViewModel
    {
        public Employee employee { get; set; }
        public List<SelectListItem> TrainingPrograms { get; set; }


        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public DetailEmployeeViewModel() { }

        public DetailEmployeeViewModel(string connectionString)
        {
            _connectionString = connectionString;

            TrainingPrograms = GetAllTraining()
                .Select(training => new SelectListItem
                {
                    Text = $"{ training.Name}",
                    Value = training.Id.ToString()
                })
                .ToList();

            TrainingPrograms.Insert(0, new SelectListItem
            {
                Text = "Choose a Training Program",
                Value = "0"
            });
        }

        private List<TrainingProgram> GetAllTraining()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], StartDate, EndDate, MaxAttendees FROM TrainingProgram";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> employees = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        employees.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("[Name]")),
                            StartDate= reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")
                            )

                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}
