using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using cw2.DTOs.Requests;
using cw2.DTOs.Responses;
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

                    // 3.Nadanie i wstawienie IdEnrollment

                    int idEnrollment;
                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdEnrollment = " +
                                            "(Select max(IdEnrollment) from Enrollment)";
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        idEnrollment = 1;
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate)" +
                        "  VALUES(" + idEnrollment + ", 1, " + idStudies + ",GetDate())";
                        com.ExecuteNonQuery();
                    }
                    idEnrollment = (int)dr["idEnrollment"];
                    dr.Close();

                    //4. Wstawienie studenta

                    string strDateFormat = "dd.MM.yyyy";
                    DateTime BirthDate = DateTime.ParseExact(request.BirthDate.ToString(), strDateFormat, CultureInfo.InvariantCulture);

                    com.CommandText = $"INSERT INTO Student VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
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
