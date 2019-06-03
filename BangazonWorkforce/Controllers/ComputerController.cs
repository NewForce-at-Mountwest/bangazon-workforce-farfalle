
//**********************************************************************************************//
// This Controller gives the client access to the Computer Resource.
// Created by Sydney Wait
//*********************************************************************************************//


using BangazonWorkforce.Models;
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

        // GET: Computers
        public ActionResult Index()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string commandText = $"SELECT Id, Make, Manufacturer FROM Computer";

                    cmd.CommandText = commandText;

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    Computer computer = null;


                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        computers.Add(computer);
                    }

                    reader.Close();

                    return View(computers);
                }
            }


        }

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



        // GET: Computer/Create
        public ActionResult Create()
        {


            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
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
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));

                        int newId = (int)cmd.ExecuteScalar();
                        computer.Id = newId;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
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

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
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
        public ActionResult Delete(int id, IFormCollection collection)
        {
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

                        //If the query comes back with no results, it has never been assigned to anyone and can be deleted safely
                        if (reader.Read())
                        {
                            doesThisComputerHaveUser = true;
                            return RedirectToAction(nameof(Delete));
                        }
                        
                    }

                    if (doesThisComputerHaveUser == false)
                    {
                        using (SqlCommand cmd2 = conn.CreateCommand())
                        {
                            cmd2.CommandText = @"DELETE FROM Computer WHERE Id = @id";
                            cmd2.Parameters.Add(new SqlParameter("@id", id));
                            int rowsAffected = cmd2.ExecuteNonQuery();
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
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
    