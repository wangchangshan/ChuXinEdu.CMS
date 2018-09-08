using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IDicQuery _dicQuery;
        private readonly IChuXinQuery _chuxinQuery;

        public StudentController(IChuXinQuery chuxinQuery, IDicQuery dicQuery)
        {
            _chuxinQuery = chuxinQuery;
            _dicQuery = dicQuery;       
        }

        /// <summary>
        /// [学生列表] 获取所有学生list GET api/student/getstudentlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<StudentListVM> GetStudentList(int pageIndex, int pageSize)
        {
            //Mapper.CreateMap<Student, StudentListVM>();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Student, StudentListVM>();
            });
            IMapper mapper = config.CreateMapper();


            IEnumerable<Student> students = _chuxinQuery.GetStudentList(pageIndex, pageSize);
            List<StudentListVM> studentVMList = new List<StudentListVM>();
            IEnumerable<Simplify_StudentCourse> studentsCourseCategory = _chuxinQuery.GetAllStudentsCourse();

            StudentListVM studentVM = null;
            foreach(Student student in students)
            {
                var studentCode = student.StudentCode;
                studentVM = mapper.Map<Student, StudentListVM>(student);
                studentVM.StudentCourseCategory = studentsCourseCategory.Where(s => s.StudentCode == studentCode);
                studentVMList.Add(studentVM);
            }

            return studentVMList;
        }

        /// <summary>
        /// [学生排课] 获取待排课学生列表 GET api/student/getstudentstoselectcourse
        /// 说明：不显示当前时间段已经选过课程的学生
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<StudentCoursePackage> GetStudentsToSelectCourse(string dayCode, string periodName)
        {
            IEnumerable<StudentCoursePackage>  studentList = _chuxinQuery.GetStudentToSelectCourse(dayCode, periodName);
            return studentList;
        }









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