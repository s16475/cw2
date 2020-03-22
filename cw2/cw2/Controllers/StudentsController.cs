using System;
using Microsoft.AspNetCore.Mvc;
using cw2.Models;
using cw2.DAL;

namespace cw2.Controllers
{
    // zadanie 3
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        // zadanie 8        
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

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
        /*
        [HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        }
        */

        //zadanie 6
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            // ... add to datbase
            // ... generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        //zadanie 7
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