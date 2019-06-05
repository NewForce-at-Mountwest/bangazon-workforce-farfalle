using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class AssignEmployeeViewModel
    {
        public Employee employee { get; set; }
        public int selectedTrainingProgramId { get; set; }
        public List<int> ThisEmployeeTrainingPrograms { get; set; }
        public List<SelectListItem> TrainingPrograms { get; set; }


        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public AssignEmployeeViewModel() { }

        public AssignEmployeeViewModel(string connectionString, int id)
        {
            _connectionString = connectionString;

            ThisEmployeeTrainingPrograms = GetThisEmployeesTrainingPrograms(id);
// compare the select list to the list of training programs already assigned

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
                    cmd.CommandText = "SELECT sq.TrainingProgramId AS 'Training Program Id', sq.[training program name] AS 'Training Program Name', sq.[employee count] as 'Employee Count', sq.MaxAttendees AS 'Max Attendees', sq.startDate as 'Start Date' FROM(select tp.id as 'trainingProgramId', tp.[Name] as 'training program name', COUNT(employeeId) as 'employee count', MaxAttendees, startDate FROM EmployeeTraining et FULL JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id GROUP BY tp.Id, tp. [Name], MaxAttendees, startDate) sq WHERE sq.[employee count] <= sq.MaxAttendees AND sq.StartDate > CURRENT_TIMESTAMP";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> TrainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                            Name = reader.GetString(reader.GetOrdinal("Training Program Name")),
                            StartDate= reader.GetDateTime(reader.GetOrdinal("Start Date")),
                            //EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("Max Attendees")
                            )

                        });
                    }

                    reader.Close();

                    return TrainingPrograms;
                }
            }
        }

        private List<int> GetThisEmployeesTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT TrainingProgramId FROM EmployeeTraining WHERE EmployeeId = {id}";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<int> ThisEmployeeTrainingPrograms = new List<int>();
                    while (reader.Read())
                    {
                        ThisEmployeeTrainingPrograms.Add(                  
                            reader.GetInt32(reader.GetOrdinal("TrainingProgramId")));
                    }

                    reader.Close();

                    return ThisEmployeeTrainingPrograms;
                }
            }
        }
    }
}
