using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cw2.DTOs.Requests;
using cw2.DTOs.Responses;
using cw2.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw2.Controllers
{
    [Route("api/enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private const string ConString = "Server=localhost;Database=master;User Id = sa; Password=B2@X7A89;";

        //zadanie 4.1 (5.1) - dodajemy nowych studentow
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse esr = new EnrollStudentResponse() { };

          
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                con.Open();
                var tran = con.BeginTransaction();
                com.Connection = con;
                com.Transaction = tran;
                try
                {

                    //1. Czy studia istnieją?
                    com.CommandText = "SELECT IdStudy AS idStudies FROM Studies WHERE Name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return NotFound("Nie ma takich studiów");

                    }

                    int idStudies = (int)dr["idStudies"];
                    dr.Close();

                    //2. Sprawdzenie czy nie występuje konflikt indeksów                  
                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = '" + request.IndexNumber + "'"; 
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Student z takim indeksem już istnieje");
                    }
                    dr.Close();

                    //3. Nadanie IdEnrollment
                    int idEnrollment;
                    com.CommandText = "SELECT IdEnrollment FROM Enrollment JOIN Studies ON " +
                        "Enrollment.IdStudy = Studies.IdStudy WHERE Semester = 1 and Enrollment.IdStudy = " + idStudies;
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "SELECT MAX(IdEnrollment)+1 AS idEnroll from Enrollment";
                        dr = com.ExecuteReader();
                        idEnrollment = (int)dr["idEnroll"];

                    }
                    else
                    {
                        idEnrollment = 1;
                        dr.Close();
                    }

                    //4. Wstawienie Enrollment                 
                    com.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate)" +
                        "  VALUES(" + idEnrollment + ", 1, " + idStudies + ",GetDate())";
                    com.ExecuteNonQuery();

                    //5. Wstawienie studenta
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) " +
                        "VALUES (" + request.IndexNumber + ", " + request.FirstName + ", " + request.LastName + ", " +
                        request.BirthDate + ", " + idEnrollment + ") ";
                    com.ExecuteNonQuery();
                    esr.IdEnrollment = idEnrollment;
                    esr.IdStudy = idStudies;
                    esr.Semester = 1;
                    esr.StartDate = DateTime.Now;
                    tran.Commit();
                    tran.Dispose();
                    return StatusCode((int)HttpStatusCode.Created, esr);

                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    return BadRequest(exc.Message);
                }
            }
        }
    }
}
