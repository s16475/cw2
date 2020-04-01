using System;
using Microsoft.AspNetCore.Mvc;
using cw2.Models;
using cw2.DAL;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace cw2.Controllers
{

    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
              
        private readonly IDbService _dbService;
        private const string ConString = "Server=localhost;Database=master;User Id = sa; Password=2X@G3382;";

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        // Zad 4.2

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<StudentInfoDTO>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s " +
                    "join Enrollment e on e.IdEnrollment = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy";
                con.Open();

                SqlDataReader dataReader = com.ExecuteReader();
                while (dataReader.Read())
                {
                    var st = new StudentInfoDTO
                    {
                        FirstName = dataReader["FirstName"].ToString(),
                        LastName = dataReader["LastName"].ToString(),
                        Name = dataReader["Name"].ToString(),
                        BirthData = dataReader["BirthDate"].ToString(),
                        Semester = dataReader["Semester"].ToString()
                    };
                    list.Add(st);
                }
            }
            return Ok(list);
        }


        // Zad. 4.3

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            var student = new StudentInfoDTO();
            using (SqlConnection connection = new SqlConnection(ConString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select s.FirstName, s.LastName, s.BirthDate, s.IndexNumber, st.Name, e.Semester from Student s " +
                    "join Enrollment e on e.IdEnrollment = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy " +
                    $"where s.IndexNumber = '{id}'";
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();

                student.FirstName = dataReader["FirstName"].ToString();
                student.LastName = dataReader["LastName"].ToString();
                student.Name = dataReader["Name"].ToString();
                student.BirthData = dataReader["BirthDate"].ToString();
                student.Semester = dataReader["Semester"].ToString();

            }

            return Ok(student);
        }

        // Zad 4.1
        /*
        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    list.Add(st);
                }
            }
            return Ok(list);
        }
        */

        public string GetStudent()
        {
            return "Kowalski, Malewski, Andrzejewski";
        }
      
        /*
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }
            return NotFound("Nie znaleziono studenta");
        }
        */


        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            // ... add to datbase
            // ... generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut]
        public IActionResult UpdateStudent(Student student)
        {
            return Ok("Zaktualizowano");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usunieto");
        }
    }
}