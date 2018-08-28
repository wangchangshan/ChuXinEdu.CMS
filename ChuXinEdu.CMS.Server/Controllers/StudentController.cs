using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;

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

        // [HttpGet("{studentcode}", Name="GetStudentByCode")]
        // public Student GetByCode(string studentCode)
        // {
        //     //  /api/student/BJ-2018070001

        //     Student student = _chuxinQuery.GetStudentByCode(studentCode);

        //     if(student == null)
        //     {

        //     }
        //     return student;
        // }

        // 数据连接测试
        [HttpGet("{studentcode}", Name = "GetStudentDescByCode")]
        public StudentDescTest GetStudentDescByCode(string studentCode)
        {
            StudentDescTest studentDesc = _chuxinQuery.GetStudentDescTest(studentCode);

            return studentDesc;
        }

    }   
}