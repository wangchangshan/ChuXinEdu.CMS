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
    }   
}