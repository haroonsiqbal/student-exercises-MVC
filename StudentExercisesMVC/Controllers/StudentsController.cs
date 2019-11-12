﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;

namespace StudentExercisesMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
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

        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT s.Id,
                                s.StuFirstName,
                                s.StuLastName,
                                s.StuSlackHandle,
                                s.CohortId
                                FROM Student s
                                ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            StuFirstName = reader.GetString(reader.GetOrdinal("StuFirstName")),
                            StuLastName = reader.GetString(reader.GetOrdinal("StuLastName")),
                            StuSlackHandle = reader.GetString(reader.GetOrdinal("StuSlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return View(students);
                }
            }
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT
                            Id, StuFirstName, StuLastName, StuSlackHandle, CohortId
                        FROM Student
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Student student = null;

                        if (reader.Read())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                StuFirstName = reader.GetString(reader.GetOrdinal("StuFirstName")),
                                StuLastName = reader.GetString(reader.GetOrdinal("StuLastName")),
                                StuSlackHandle = reader.GetString(reader.GetOrdinal("StuSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            };
                        }
                        reader.Close();

                        return View(student);
                    }
                }
            }
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
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

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                            SET StuFirstName = @stuFirstName,
                                                StuLastName = @stuLastName,
                                                StuSlackHandle = @stuSlackHandle,
                                                CohortId = @cohortId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@stuFirstName", student.StuFirstName));
                        cmd.Parameters.Add(new SqlParameter("@stuLastName", student.StuLastName));
                        cmd.Parameters.Add(new SqlParameter("@stuSlackHandle", student.StuSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsaffected = cmd.ExecuteNonQuery();
                        if (rowsaffected > 0)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        throw new Exception("no rows affected");
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";
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
            catch
            {
                return View();
            }
        }
    }
}