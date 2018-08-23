using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // private readonly StudentController _context;

        // public StudentController(StudentController context)
        // {
        //     _context = context;
        // }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetAll()
        {
            //  /api/student           
            Student[] studentAll = null;
            using (var context = new BaseContext())
            {
                studentAll =  context.Student.ToList().ToArray();
            }
            return studentAll;
        }

        // [HttpGet("{studentcode}", Name="GetStudentByCode")]
        // public ActionResult<Student> GetByCode(string studentCode)
        // {
        //     var item = _context.Student.Where(s => s.StudentCode == studentCode);

        //     if(item == null)
        //     {
        //         return NotFound();
        //     }
        //     return item;
        // }

        [HttpGet("{studentcode}", Name="GetStudentByCode")]
        public List<Student> GetByCode(string studentCode)
        {
            //  /api/student/BJ-2018070001
            List<Student> item = null;
            using (var context = new BaseContext())
            {
                item =  context.Student.Where(s => s.StudentCode == studentCode).ToList();
            }

            if(item == null)
            {
                //return NotFound();
            }
            return item;
        }
    }   
}