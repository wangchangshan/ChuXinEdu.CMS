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

        /// <summary>
        /// 获取所有学生list GET api/student
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Student> GetALL() => _chuxinQuery.GetAllStudents();


        // /// <summary>
        // /// 通过学生姓名获取学生list GET api/student/怀远
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet("{studentname}", Name="GetFiltered")]
        // public IEnumerable<Student> GetFiltered(string studentName)
        // {
        //     return _chuxinQuery.GetStudentsByName(studentName);
        // }

        /// <summary>
        /// 获取学生基础信息 GET api/student/BJ-2018070002/baseinfo
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentcode}")]
        [Route("{studentcode}/baseinfo")]
        public Student GetBaseInfo(string studentCode)
        {
            Student student = new Student();
            student = _chuxinQuery.GetStudentBaseByCode(studentCode);
            return student;
        }

        /// <summary>
        /// 获取学生基础信息 GET api/student/BJ-2018070002/history
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentcode}")]
        [Route("{studentcode}/history")]
        public Student GetHistory(string studentCode)
        {
            Student student = new Student();
            return student;
        }




        /// <summary>
        /// 添加新学生 POST api/student
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void Create([FromBody] string value)
        {
        }

        /// <summary>
        /// 更新学生 PUT api/student/5
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public void Update(int id, [FromBody] string value)
        {
        }



        // // 数据连接测试
        // [HttpGet("{studentcode}", Name = "GetStudentDescByCode")]
        // public StudentDescTest GetStudentDescByCode(string studentCode)
        // {
        //     StudentDescTest studentDesc = _chuxinQuery.GetStudentDescTest(studentCode);

        //     return studentDesc;
        // }

    }   
}