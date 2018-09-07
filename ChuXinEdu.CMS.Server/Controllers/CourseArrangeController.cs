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
    public class CourseArrangeController : ControllerBase
    {
        private readonly IChuXinWorkFlow _chuxinWorkFlow;
        private readonly IChuXinQuery _chuxinQuery;

        public CourseArrangeController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取排课所有时间段 GET api/coursearrange/getcoursearranged
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
        /// 提交排课信息 POST api/coursearrange/postcoursearrange
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string PostCourseArrange([FromBody] CA_C_STUDENTS_MAIN caInfo)
        {
            string result = _chuxinWorkFlow.BatchStudentsCourseArrange(caInfo);
            return result;
        }

        /// <summary>
        /// 课程个人请假 PUT api/coursearrange/putqingjiasingle
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutQingJiaSingle(dynamic obj)
        {
            int studentCourseId = obj.StudentCourseId;
            string result = _chuxinWorkFlow.SingleQingJia(studentCourseId);

            return result;
        }

        /// <summary>
        /// 删除个人课程 PUT api/coursearrange/removeCourse
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string RemoveCourse(dynamic obj)
        {
            string result = "";

            return result;
        }
    }   
}