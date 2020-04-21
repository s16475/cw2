using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw2.DTOs.Requests;
using cw2.DTOs.Responses;
using cw2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cw2.Controllers
{
    [Route("api/enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private const string ConString = "Server=localhost;Database=master;User Id = sa; Password=B2@X7A89;";


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //zadanie 4.1 (5.1) - dodajemy nowych studentow
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.FirstName = request.FirstName;

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    //1. Czy studia istnieja?
                    com.CommandText = "select IdStudies from studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Studis nie istnieja");
                    }
                    int idstudies = (int)dr["IdStudies"];

                    //3. Dodanie studenta
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName VALUES(@Index,@Fname";
                    com.Parameters.AddWithValue("Index", request.IndexNumber);
                    com.ExecuteNonQuery();

                    tran.Commit();
                } catch(SqlException e)
                {
                    tran.Rollback();
                    return BadRequest(e);
                }
            } 

            var response = new EnrollStudentResponse();
            response.LastName = st.LastName;

            return Ok(response);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
