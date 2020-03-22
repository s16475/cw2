using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw2.Controllers
{
    // zadanie 3
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        
        //GetStudent() - zad 3
        public string GetStudent()
        {
            return "Kowalski, Malewski, Andrzejewski";
        }
      
        // zadanie 4
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

        //GetStudents() - zad 5
        [HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        }





    }
}