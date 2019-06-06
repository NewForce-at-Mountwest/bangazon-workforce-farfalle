using BangazonWorkforce.Models.SubModels;
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
        private HashSet<SimpleTraining> ThisEmployeeTrainingPrograms { get; set; } = new HashSet<SimpleTraining>(new TrainingComparer());
        private HashSet<SimpleTraining> AllFutureAvailableTrainingPrograms { get; set; } = new HashSet<SimpleTraining>(new TrainingComparer());

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


            AllFutureAvailableTrainingPrograms = GetAllTraining();

            AllFutureAvailableTrainingPrograms.ExceptWith(ThisEmployeeTrainingPrograms);



            // compare the select list to the list of training programs already assigned

            TrainingPrograms = AllFutureAvailableTrainingPrograms
                .Select(training => new SelectListItem
                {
                    Text = $"{training.Name}",
                    Value = training.Id.ToString()
                })
                .ToList();

            TrainingPrograms.Insert(0, new SelectListItem
            {
                Text = "Choose a Training Program",
                Value = "0"
            });


        }

        private HashSet<SimpleTraining> GetAllTraining()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT sq.TrainingProgramId AS 'Training Program Id', sq.[training program name] AS 'Training Program Name', sq.[employee count] as 'Employee Count', sq.MaxAttendees AS 'Max Attendees', sq.startDate as 'Start Date' FROM(select tp.id as 'trainingProgramId', tp.[Name] as 'training program name', COUNT(employeeId) as 'employee count', MaxAttendees, startDate FROM EmployeeTraining et FULL JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id GROUP BY tp.Id, tp. [Name], MaxAttendees, startDate) sq WHERE sq.[employee count] <= sq.MaxAttendees AND sq.StartDate > CURRENT_TIMESTAMP";
                    SqlDataReader reader = cmd.ExecuteReader();

                    HashSet<SimpleTraining> TrainingPrograms = new HashSet<SimpleTraining>(new TrainingComparer());
                    while (reader.Read())
                    {
                        TrainingPrograms.Add(new SimpleTraining
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                            Name = reader.GetString(reader.GetOrdinal("Training Program Name"))
                           
                            
                        });
                    }

                    reader.Close();

                    return TrainingPrograms;
                }
            }
        }

        private HashSet<SimpleTraining> GetThisEmployeesTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT tp.id, tp.name FROM EmployeeTraining et JOIN TrainingProgram tp ON tp.id = et.TrainingProgramId WHERE employeeId={id}";
                    SqlDataReader reader = cmd.ExecuteReader();

                    HashSet<SimpleTraining> ThisEmployeeTrainingPrograms = new HashSet<SimpleTraining>(new TrainingComparer());


                    while (reader.Read())
                    {

                        ThisEmployeeTrainingPrograms.Add(new SimpleTraining
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))


                        });                   
                        
                                               
                        }
                reader.Close();

                    return ThisEmployeeTrainingPrograms;
                }
            }
        }
    }

    public class TrainingComparer : IEqualityComparer<SimpleTraining>
    {
        public bool Equals(SimpleTraining x, SimpleTraining y)
        {
            return x.Id==y.Id;
        }

        public int GetHashCode(SimpleTraining obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
