
//**********************************************************************************************//
// This Controller gives the client access to the Computer Resource.
// Created by Sydney Wait
//*********************************************************************************************//


using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Controllers
{

    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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
        /// <summary>
        /// This method returns a list of all the computers in the database
        /// </summary>
        /// <returns>List<Computer></returns>
        // GET: Computers
        public ActionResult Index(string searchString)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string commandText = $"SELECT c.Id as 'Computer Id', c.Make, c.Manufacturer, e.id as 'Employee Id', e.FirstName, e.LastName, ce.AssignDate, ce.UnassignDate FROM Computer c LEFT JOIN ComputerEmployee ce ON CE.ComputerId = c.Id LEFT JOIN Employee e ON ce.employeeId=e.Id ";


                    if (!String.IsNullOrEmpty(searchString))
                    {
                        commandText += $" WHERE Make LIKE '%{searchString}%' OR Manufacturer LIKE '%{searchString}%' ORDER BY AssignDate DESC";
                    }
                    else
                    {
                        commandText += $" ORDER BY AssignDate DESC";
                    }

                    cmd.CommandText = commandText;

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    Computer computer = null;
                    Employee employee = null;


                    while (reader.Read())
                    {


                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Computer Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        //Check to see if the computer is already on the computers list.  If it is, make sure that you are getting the one that is currently assigned, and not the one that is unassigned. 
                        //Also check to make sure that the computer that is added isn't one that is unassigned
                        if (!computers.Any(c => c.Id == computer.Id)) {
                            Computer computerOnList = computers.Where(s => s.Id == computer.Id).FirstOrDefault();


                            if (reader.IsDBNull(reader.GetOrdinal("UnassignDate")) && !reader.IsDBNull(reader.GetOrdinal("assignDate"))) { 
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                        computer.CurrentEmployee = employee;
                        }

                        computers.Add(computer);
                        }

                    }

                    reader.Close();

                    return View(computers);
                }
            }


        }
        /// <summary>
        /// This method gets the details of the computer the user has selected to view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string commandText = $"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer FROM Computer WHERE Id={id}";

                    cmd.CommandText = commandText;

                    SqlDataReader reader = cmd.ExecuteReader();
                    Computer computer = null;


                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                        //Make sure the decomission date is not null before getting the value
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }
                        else
                        {
                            computer.DecomissionDate = null;

                        }

                    }

                    reader.Close();

                    return View(computer);

                }
            }
        }


        /// <summary>
        /// This auto-generates the form to add a computer to the database based on the Computer model
        /// </summary>
        /// <returns></returns>
        // GET: Computer/Create
        public ActionResult Create()
        {
            //Creates a new instance based on the view model
            CreateComputerViewModel computerViewModel = new CreateComputerViewModel
                (_config.GetConnectionString("DefaultConnection"));
            //Pass it to the view
            return View(computerViewModel);
        }
        /// <summary>
        /// This posts a new computer to the database
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>A new computer to be posted</returns>
        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerViewModel computerViewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer (make, manufacturer, purchaseDate, decomissionDate)
                                                OUTPUT INSERTED.Id
                                                VALUES (@make, @manufacturer, @purchaseDate, null)";
                        cmd.Parameters.Add(new SqlParameter("@make", computerViewModel.computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computerViewModel.computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computerViewModel.computer.PurchaseDate));

                        int newId = (int)cmd.ExecuteScalar();
                        computerViewModel.computer.Id = newId;

                        /// <summary>If the user selects an employee to whom they want to assign the computer, assign and unassign old</summary>
                        if (computerViewModel.employeeId != 0) 
                            {
                                cmd.CommandText = @"INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
                                                OUTPUT INSERTED.Id
                                                VALUES (@employeeId, @computerId, @assignDate, null)";
                                cmd.Parameters.Add(new SqlParameter("@employeeId", computerViewModel.employeeId));
                                cmd.Parameters.Add(new SqlParameter("@computerId", newId));
                                cmd.Parameters.Add(new SqlParameter("@assignDate", DateTime.Now));


                                int newCEId = (int)cmd.ExecuteScalar();

                                cmd.CommandText = @"UPDATE ComputerEmployee SET UnassignDate = @unassignDate WHERE employeeID = @employeeId AND computerId != @computerId";

                                cmd.Parameters.Add(new SqlParameter("@unassignDate", DateTime.Now));

                                cmd.ExecuteScalar();
                            }


                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

         
    

        /// <summary>
        /// This method populates the page with the selected computer to delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Computer to be deleted</returns>
        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            ///<summary>Check to see if there are any error messages to display</summary>
            var errMsg = TempData["ErrorMessage"] as string;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string commandText = @"
                             SELECT c.Id, c.make, c.manufacturer, c.purchaseDate, c.decomissionDate FROM Computer c
                             WHERE Id=@Id";


                    cmd.CommandText = commandText;


                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computerToDelete = null;

                    while (reader.Read())
                    {

                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate")),
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("decomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("decomissionDate"));
                        }
                        else
                        {
                            computer.DecomissionDate = null;
                        }

                        computerToDelete = computer;
                    }
                    reader.Close();

                    return View(computerToDelete);
                }
            }
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Computer computer)
        {
            ///<summary>Check to see if there are any error messages to display</summary>
            var errMsg = TempData["ErrorMessage"] as string;

            try
            {
                using (SqlConnection conn = Connection)
                {
                    bool doesThisComputerHaveUser = false;
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        string commandText = $"SELECT c.id AS 'Computer ID', e.id AS 'employeeId' FROM ComputerEmployee ce JOIN Employee e ON e.Id = ce.EmployeeId JOIN Computer c ON c.id = ce.ComputerId WHERE c.id = {id}";

                        cmd.CommandText = commandText;
                        SqlDataReader reader = cmd.ExecuteReader();

                        /// <summary>If the query comes back with results, the computer has been assigned and cannot be deleted</summary>
                        if (reader.Read())
                        {
                            doesThisComputerHaveUser = true;
                            throw new Exception("This computer has a user");
                        }
                        reader.Close();

                    }
                    /// <summary>If the query comes back with no results, the computer has not been assigned and can be safely deleted</summary>

                    if (doesThisComputerHaveUser == false)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception("No rows affected");

                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ///<summary>Display an error message if computer cannot be deleted</summary>
                TempData["ErrorMessage"] = "This computer cannot be deleted because it is currently or previously assigned to an employee";
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}
    