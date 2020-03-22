using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw2.Models;

namespace cw2.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
           

    }
}
