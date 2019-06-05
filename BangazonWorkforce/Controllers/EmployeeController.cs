using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;

namespace BangazonWorkforce.Controllers
{
    //Employee controller for mvc views, authored by Sable Bowen
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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


        // GET: Employee
        public ActionResult Index()
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        //joins employee, department, and computer tables
                        string command = $@"SELECT e.Id AS 'Employee Id', e.FirstName, e.LastName, e.DepartmentId,
                        d.Id AS 'Department Id', d.Name AS 'Department'
                        FROM Employee e FULL JOIN Department d ON e.DepartmentId = d.Id";

                        cmd.CommandText = command;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Employee> employees = new List<Employee>();

                        while (reader.Read())
                        {


                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                CurrentDepartment = new Department()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Department")),
                                    
                                }
                            };

                        
                    
                           


                            employees.Add(employee);
                        }
                        reader.Close();
                        return View(employees);
                    }
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            {
                using (SqlConnection conn = Connection)
                {

                    
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT e.Id AS 'Employee Id', e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId,
                        d.Id AS 'Department Id', d.Name AS 'Department', d.Budget ,c.Id AS 'Computer Id', tp.Name AS 
						ProgramName,
						c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate, tp.Id AS 'Training Id'
                        FROM Employee e LEFT JOIN Department d ON e.DepartmentId = d.Id
						LEFT JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
                        LEFT JOIN Computer c ON ce.ComputerId=c.Id LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId 
						LEFT JOIN 
						TrainingProgram tp 
						ON et.TrainingProgramId = tp.Id WHERE e.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Employee employee = null;

                        while (reader.Read())
                        {

                            if (employee == null)
                            {
                                employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    CurrentDepartment = new Department()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Department")),
                                    },

                                    CurrentComputer = null,
                                    TrainingPrograms = new List<TrainingProgram>()

                                };
                            }


                            if (!reader.IsDBNull(reader.GetOrdinal("Training Id"))) {
                                TrainingProgram trainingProgram = new TrainingProgram()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Training Id")),
                                    Name = reader.GetString(reader.GetOrdinal("ProgramName"))
                                };

                                employee.TrainingPrograms.Add(trainingProgram);
                            }



                            if (!reader.IsDBNull(reader.GetOrdinal("Computer Id")))
                            {
                                Computer computer = new Computer()
                                {
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                };
                                employee.CurrentComputer = computer;
                            }


                        }
                        reader.Close();

                       
                        return View(employee);

                    }
                }
            }
        }
            // GET: Employee/Create
            public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Assign/5
        public ActionResult Assign(int id)
        {
            AssignEmployeeViewModel assignView = new AssignEmployeeViewModel(_config.GetConnectionString("DefaultConnection"));


            return View(assignView);
        }

        // POST: Employee/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}