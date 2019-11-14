using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

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
            var viewModel = new StudentCreateViewModel();
            var cohorts = GetAllCohorts();
            var selectItems = cohorts
                .Select(cohort => new SelectListItem
                {
                    Text = cohort.CohortName,
                    Value = cohort.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
            viewModel.Cohorts = selectItems;
            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Student
                ( StuFirstName, StuLastName, StuSlackHandle, CohortId )
                VALUES
                ( @firstName, @lastName, @slackHandle, @cohortId )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.student.StuFirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.student.StuLastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", model.student.StuSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", model.student.CohortId));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
 

               
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new StudentEditViewModel()
            {
                student = GetStudentById(id),
                cohorts = GetAllCohorts()

            };
            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, StudentEditViewModel viewModel)
        {
            var updatedStudent = viewModel.student;
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
                        cmd.Parameters.Add(new SqlParameter("@stuFirstName", updatedStudent.StuFirstName));
                        cmd.Parameters.Add(new SqlParameter("@stuLastName", updatedStudent.StuLastName));
                        cmd.Parameters.Add(new SqlParameter("@stuSlackHandle", updatedStudent.StuSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", updatedStudent.CohortId));
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
                viewModel = new StudentEditViewModel()
                {
                    student = updatedStudent,
                    cohorts = GetAllCohorts()

                };
                return View(viewModel);
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

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, CohortName FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                        });
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }

        private Student GetStudentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.StuFirstName, s.StuLastName, 
                                               s.StuSlackHandle, s.CohortId,
                                               c.CohortName
                                         FROM Student s INNER JOIN Cohort c on c.Id = s.CohortId
                                        WHERE s.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Student student = null;
                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            StuFirstName = reader.GetString(reader.GetOrdinal("StuFirstname")),
                            StuLastName = reader.GetString(reader.GetOrdinal("StuLastName")),
                            StuSlackHandle = reader.GetString(reader.GetOrdinal("StuSlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }
                    reader.Close();
                    return student;
                }
            }
        }

    }
}