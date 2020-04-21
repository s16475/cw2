using System;
using System.ComponentModel.DataAnnotations;

namespace cw2.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(10)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        [Required]
        public string Studies { get; set; }
    }
}
