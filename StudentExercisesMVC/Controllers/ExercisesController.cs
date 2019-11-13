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
    public class ExercisesController : Controller
    {
        private readonly IConfiguration _config;

        public ExercisesController(IConfiguration config)
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

        // GET: Exercises
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT e.Id,
                                e.ExerciseName,
                                e.ExerciseLang                            
                                FROM Exercise e
                                ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLang = reader.GetString(reader.GetOrdinal("ExerciseLang"))
                        };

                        exercises.Add(exercise);
                    }

                    reader.Close();

                    return View(exercises);
                }
            }
        }

        // GET: Exercises/Details/5
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
                            Id, ExerciseName, ExerciseLang
                        FROM Exercise
                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Exercise exercise = null;

                        if (reader.Read())
                        {
                            exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                ExerciseLang = reader.GetString(reader.GetOrdinal("ExerciseLang"))
                               
                            };
                        }
                        reader.Close();

                        return View(exercise);
                    }
                }
            }
        }

        // GET: Exercises/Create
        public ActionResult Create()
        {
            //var viewModel = new StudentCreateViewModel();
            //var cohorts = GetAllCohorts();
            //var selectItems = cohorts
            //    .Select(cohort => new SelectListItem
            //    {
            //        Text = cohort.CohortName,
            //        Value = cohort.Id.ToString()
            //    })
            //    .ToList();

            //selectItems.Insert(0, new SelectListItem
            //{
            //    Text = "Choose cohort...",
            //    Value = "0"
            //});
            //viewModel.Cohorts = selectItems;
            return View();
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Exercise
                ( ExerciseName, ExerciseLang)
                VALUES
                ( @exerciseName, @exerciseLang)";
                        cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@exerciseLang", exercise.ExerciseLang));
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

        // GET: Exercises/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection, Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET ExerciseName = @exerciseName,
                                                ExerciseLang = @exerciseLang
                                                
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@exerciseLang", exercise.ExerciseLang));
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

        // GET: Exercises/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Exercises/Delete/5
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
                            cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";
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

    }
}