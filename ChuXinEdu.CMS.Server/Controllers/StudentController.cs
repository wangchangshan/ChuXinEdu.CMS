using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;

        public StudentController(IChuXinQuery chuxinQuery)
        {
            _chuxinQuery = chuxinQuery;            
        }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetAll()
        {
            //  /api/student           
            // Student[] studentAll = null;
            // using (var context = new BaseContext())
            // {
            //     studentAll =  context.Student.ToList().ToArray();
            // }
            // return studentAll;

            return _chuxinQuery.GetAllStudents().ToArray();
        }

        // [HttpGet("{studentcode}", Name="GetStudentByCode")]
        // public IEnumerable<Student> GetByCode(string studentCode)
        // {
        //     //  /api/student/BJ-2018070001
        //     List<Student> item = null;
        //     using (var context = new BaseContext())
        //     {
        //         item =  context.Student.Where(s => s.StudentCode == studentCode).ToList();
        //     }

        //     if(item == null)
        //     {
        //         //return NotFound();
        //     }
        //     return item;
        // }

        [HttpGet("{studentcode}", Name="GetStudentByCode")]
        public Student GetByCode(string studentCode)
        {
            Student student = _chuxinQuery.GetStudentByCode(studentCode);

            return student;
        }
    }   
}