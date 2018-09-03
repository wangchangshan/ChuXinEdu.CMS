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
    [Route("api/[controller]")]
    [ApiController]
    public class CourseArrangeController : ControllerBase
    {
        private readonly IDicQuery _dicQuery;
        private readonly IChuXinQuery _chuxinQuery;

        public CourseArrangeController(IChuXinQuery chuxinQuery, IDicQuery dicQuery)
        {
            _chuxinQuery = chuxinQuery;
            _dicQuery = dicQuery;       
        }

        /// <summary>
        /// 获取排课所有时间段 GET api/coursearrange
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CourseArrangeVM> GetCourseArranged(string templateCode, string roomCode)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<SysCourseArrangeTemplateDetail, CourseArrangeVM>();
            });
            IMapper mapper = config.CreateMapper();

            // 获取每天上课时间段
            IEnumerable<SysCourseArrangeTemplateDetail> periodsList = _chuxinQuery.GetCourseArrangePeriod(templateCode);

            // 获取学生选课信息（包含每个时间段）
            IEnumerable<StudentCourseArrange> studentCourseArrangeList = _chuxinQuery.GetStudentCourseArrage(templateCode, roomCode);

            // 定义返回数据
            List<CourseArrangeVM> courseArrangeVMList = new List<CourseArrangeVM>();
            CourseArrangeVM courseArrangeVM = null;
            foreach(SysCourseArrangeTemplateDetail periods in periodsList)
            {
                var period = periods.CoursePeriod;
                var day = periods.CourseWeekDay;

                courseArrangeVM = mapper.Map<SysCourseArrangeTemplateDetail, CourseArrangeVM>(periods);
                courseArrangeVM.StudentCourseArrangeList = studentCourseArrangeList.Where(s => s.CourseWeekDay == day && s.CoursePeriod == period);

                courseArrangeVMList.Add(courseArrangeVM);
            }             

            return courseArrangeVMList;
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