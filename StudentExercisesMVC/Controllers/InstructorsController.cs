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
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
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

        // GET: Instructors
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT i.Id,
                                i.InstFirstName,
                                i.InstLastName,
                                i.InstSlackHandle,
                                i.InstCohort,
                                i.InstSpeciality
                                FROM Instructor i
                                ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InstFirstName = reader.GetString(reader.GetOrdinal("InstFirstName")),
                            InstLastName = reader.GetString(reader.GetOrdinal("InstLastName")),
                            InstSlackHandle = reader.GetString(reader.GetOrdinal("InstSlackHandle")),
                            InstCohort = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                            InstSpeciality = reader.GetString(reader.GetOrdinal("InstSpeciality")),

                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return View(instructors);
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
                            Id, InstFirstName, InstLastName, InstSlackHandle, InstCohort, InstSpeciality
                        FROM Instructor
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Instructor instructor = null;

                        if (reader.Read())
                        {
                            instructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                InstFirstName = reader.GetString(reader.GetOrdinal("InstFirstName")),
                                InstLastName = reader.GetString(reader.GetOrdinal("InstLastName")),
                                InstSlackHandle = reader.GetString(reader.GetOrdinal("InstSlackHandle")),
                                InstCohort = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                                InstSpeciality = reader.GetString(reader.GetOrdinal("InstSpeciality"))
                            };
                        }
                        reader.Close();

                        return View(instructor);
                    }
                }
            }
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            var viewModel = new InstructorCreateViewModel();
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
        public ActionResult Create(InstructorCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Instructor
                ( InstFirstName, InstLastName, InstSlackHandle, InstCohort, InstSpeciality )
                VALUES
                ( @instFirstName, @instLastName, @instSlackHandle, @instCohort, @instSpeciality )";
                        cmd.Parameters.Add(new SqlParameter("@instFirstName", model.instructor.InstFirstName));
                        cmd.Parameters.Add(new SqlParameter("@instLastName", model.instructor.InstLastName));
                        cmd.Parameters.Add(new SqlParameter("@instSlackHandle", model.instructor.InstSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@instCohort", model.instructor.InstCohort));
                        cmd.Parameters.Add(new SqlParameter("@instSpeciality", model.instructor.InstSpeciality));
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
            var viewModel = new InstructorEditViewModel()
            {
                instructor = GetInstructorById(id),
                cohorts = GetAllCohorts()

            };
            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, InstructorEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Instructor
                                            SET InstFirstName = @instFirstName,
                                                InstLastName = @instLastName,
                                                InstSlackHandle = @instSlackHandle,
                                                InstCohort = @instCohort,
                                                InstSpeciality = @instSpeciality
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@instFirstName", viewModel.instructor.InstFirstName));
                        cmd.Parameters.Add(new SqlParameter("@instLastName", viewModel.instructor.InstLastName));
                        cmd.Parameters.Add(new SqlParameter("@instSlackHandle", viewModel.instructor.InstSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@instCohort", viewModel.instructor.InstCohort));
                        cmd.Parameters.Add(new SqlParameter("@instSpeciality", viewModel.instructor.InstSpeciality));
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
                            cmd.CommandText = @"DELETE FROM Instructor WHERE Id = @id";
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
        private Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.InstFirstName, i.InstLastName, 
                                               i.InstSlackHandle, i.InstCohort, i.InstSpeciality,
                                               c.CohortName
                                         FROM Instructor i INNER JOIN Cohort c on c.Id = i.InstCohort
                                        WHERE i.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Instructor instructor = null;
                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            InstFirstName = reader.GetString(reader.GetOrdinal("InstFirstname")),
                            InstLastName = reader.GetString(reader.GetOrdinal("InstLastName")),
                            InstSlackHandle = reader.GetString(reader.GetOrdinal("InstSlackHandle")),
                            InstCohort = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                            InstSpeciality = reader.GetString(reader.GetOrdinal("InstSpeciality")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("InstCohort")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }
                    reader.Close();
                    return instructor;
                }
            }
        }
    }
}