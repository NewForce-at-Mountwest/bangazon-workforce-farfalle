using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;

namespace BangazonWorkforce.Controllers
{
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
                        string command = $@"SELECT e.Id AS 'Employee Id', e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId,
                        d.Id AS 'Department Id', d.Name AS 'Department', d.Budget ,c.Id AS 'Computer Id', 
						c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate
                        FROM Employee e FULL JOIN Department d ON e.DepartmentId = d.Id
						LEFT JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
                        LEFT JOIN Computer c ON ce.ComputerId=c.Id";

                        cmd.CommandText = command;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Employee> employees = new List<Employee>();

                        while (reader.Read())
                        {

                            //currentcomputer will default to null, because...

                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                CurrentDepartment = new Department()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Department")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                },
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                CurrentComputer = null

                            };
                            //if the reader finds a value for an employee under the computer id column, it will attach the computer to the employee under their currentcomputer
                            if (!reader.IsDBNull(reader.GetOrdinal("Computer Id")))
                            {
                                Computer computer = new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Computer Id")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),

                                };
                                employee.CurrentComputer = computer;
                            }

                            //this checks to see if the decomissiondate for a computer exists, if it does, it sets the date on the computer

                            if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                employee.CurrentComputer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));

                            }


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
            return View();
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
    }
}