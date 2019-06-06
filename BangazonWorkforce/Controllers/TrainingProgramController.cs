using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    ///  Controller for training programs. Full crud, but there's a second GET all for past trainings. 
    ///  By Connor FitzGerald
    
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET ALL TRAINING PROGRAMS THAT START AFTER TODAY'S DATE
       
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT t.Id,
                t.Name,
                t.StartDate,
                t.EndDate,
                t.MaxAttendees
            FROM TrainingProgram t
        ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        ///This will only add trainings to the display list if the start date is after today
                            DateTime now = DateTime.Now;
                            if (!(DateTime.Compare(trainingProgram.StartDate, now) < 0))
                            {
                                trainingPrograms.Add(trainingProgram);
                            }
                    }

                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }

        //This is a list of past trainings
        public ActionResult List()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT t.Id,
                t.Name,
                t.StartDate,
                t.EndDate,
                t.MaxAttendees
            FROM TrainingProgram t
        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                        //Only adds trainings that have already started
                        DateTime now = DateTime.Now;
                        if ((DateTime.Compare(trainingProgram.StartDate, now) < 0))
                        {
                            trainingPrograms.Add(trainingProgram);
                        }
                    }

                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }

        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT t.Id as 'trainingId', 
                                        t.Name, t.StartDate, t.EndDate, 
                                        t.MaxAttendees, e.Id AS 'Employee Id', e.FirstName, 
                                        e.LastName, e.DepartmentId FROM EmployeeTraining et
                                        FULL JOIN Employee e on et.EmployeeId = e.id 
                                        FULL JOIN TrainingProgram t on et.TrainingProgramId = t.id WHERE t.Id = @id";
                                       

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingToDisplay = null;
                    while (reader.Read())
                    {
                        if (trainingToDisplay == null)
                        {
                            trainingToDisplay = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("trainingId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                Employees = new List<Employee>()
                            };
                        };
                        //adds an employee if it exists to the trainings employee list
                        if (!reader.IsDBNull(reader.GetOrdinal("Employee Id")))
                        {
                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                               
                            };
                            trainingToDisplay.Employees.Add(employee);
                        }
                    }
                    reader.Close();

                    return View(trainingToDisplay);
                }
            }
        }

        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TrainingProgram trainingProgram)
        {
                using (SqlConnection conn = Connection)
                {
                if (ModelState.IsValid)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram
                (Name, StartDate, EndDate, MaxAttendees)
                VALUES
                (@Name, @StartDate, @EndDAte, @MaxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                        }
                    }else{
                    return View();
                    }
                 
                }
        }

        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT 
                t.Id, t.Name, t.StartDate, t.EndDate, t.MaxAttendees
            FROM TrainingProgram t 
            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram training = null;
                    if (reader.Read())
                    {
                        training = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                        };
                    }
                    reader.Close();
                    //hopefully makes it so you can't edit past trainings 
                    //but there shouldn't be a way to naturally navigate to edit for a past training
                    DateTime now = DateTime.Now;
                    if (!(DateTime.Compare(training.EndDate, now) < 0))
                    {
                        return View(training);
                    }
                    else {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
        }

        // POST: Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram training)
        {
          using (SqlConnection conn = Connection)
                {
                //validates that the end date is not earlier than the start date
                    if (ModelState.IsValid)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE TrainingProgram
                                            SET Name = @Name, 
                                            StartDate = @StartDate,
                                            EndDate = @EndDate,
                                            MaxAttendees = @MaxAttendees
                                            WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@Name", training.Name));
                            cmd.Parameters.Add(new SqlParameter("@StartDate", training.StartDate));
                            cmd.Parameters.Add(new SqlParameter("@EndDate", training.EndDate));
                            cmd.Parameters.Add(new SqlParameter("@MaxAttendees", training.MaxAttendees));

                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            training = new TrainingProgram();

                            int rowsAffected = cmd.ExecuteNonQuery();

                            return RedirectToAction(nameof(Index));

                        }
                    }
                    else
                    {
                        return View(training);
                    }

                    }
                }
            
                
         

        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT s.Id,
                s.Name,
                s.StartDate,
                s.EndDate,  
                s.MaxAttendees
            FROM TrainingProgram s 
            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram= new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                        };
                    }

                    reader.Close();

                    return View(trainingProgram);
                }
            }
        }

        // POST: Cohorts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    throw new Exception("No rows affected");
                }
            }
        }
    }
}