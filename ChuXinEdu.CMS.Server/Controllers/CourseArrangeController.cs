using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class CourseArrangeController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

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
            IEnumerable<CA_R_PERIOD_STUDENTS> periodStudents = _chuxinQuery.GetAllPeriodStudents(templateCode, roomCode);

            // 定义返回数据
            List<CourseArrangeVM> courseArrangeVMList = new List<CourseArrangeVM>();
            CourseArrangeVM courseArrangeVM = null;
            foreach(SysCourseArrangeTemplateDetail periods in periodsList)
            {
                var period = periods.CoursePeriod;
                var day = periods.CourseWeekDay;

                courseArrangeVM = mapper.Map<SysCourseArrangeTemplateDetail, CourseArrangeVM>(periods);
                courseArrangeVM.ThisWeekStudentCount = periodStudents.Where(s => s.CourseWeekDay == day && s.CoursePeriod == period && s.IsThisWeek == "Y").Count();
                courseArrangeVM.PeriodStudentList = periodStudents.Where(s => s.CourseWeekDay == day && s.CoursePeriod == period);

                courseArrangeVMList.Add(courseArrangeVM);
            }             

            return courseArrangeVMList;
        }

        /// <summary>   
        /// 获取上课时间段 GET api/coursearrange/getpriodlist
        /// </summary>
        /// <returns></returns>
        [HttpGet("{templateCode}")]
        public IEnumerable<SysCourseArrangeTemplateDetail> GetPriodList(string templateCode)
        {
            return _chuxinQuery.GetCourseArrangePeriod(templateCode);
        }

        /// <summary>   
        /// 获取今天排课信息 GET api/coursearrange/getcoursearrangedtoday
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<StudentCourseList> GetCourseArrangedToday()
        {
            string strGuohua = string.Empty;
            string strXihua = string.Empty;
            string strShufa = string.Empty;

            List<StudentCourseList> scls = _chuxinQuery.GetCoursesToday();        
            return scls;
        }

        /// <summary>
        /// 根据时间段获取排课信息，用于局部刷新 GET api/coursearrange/getarrangedinfobyperiod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<CA_R_PERIOD_STUDENTS> GetArrangedInfoByPeriod(string templateCode, string roomCode, string dayCode, string periodName)
        {
            IEnumerable<CA_R_PERIOD_STUDENTS> arrangedInfo  = _chuxinQuery.GetPeriodStudents(templateCode, roomCode, dayCode, periodName);
            return arrangedInfo;
        }

        /// <summary>
        /// 根据时间段获取排课信息, GET api/coursearrange/getholidays
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SysHoliday> GetHolidays()
        {
            IEnumerable<SysHoliday> holidays = _chuxinQuery.GetHolidayList();
            return holidays;
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
        /// 个人请假撤销 PUT api/coursearrange/putrestoreqingjiasingle
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutRestoreQingJiaSingle(dynamic obj)
        {
            int studentCourseId = obj.StudentCourseId;
            string result = _chuxinWorkFlow.RestoreSingleQingJia(studentCourseId);

            return result;
        }

        /// <summary>
        /// 删除个人课程 PUT api/coursearrange/removecoursesingle
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string RemoveCourseSingle(dynamic obj)
        {
            int studentCourseId = obj.StudentCourseId;
            string result = _chuxinWorkFlow.SingleRemoveCourse(studentCourseId);
            return result;
        }

        /// <summary>
        /// 批量删除个人课程 PUT api/coursearrange/removecoursebatch
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string RemoveCourseBatch([FromBody] int[] studentCourseIds)
        {
            string result = string.Empty;
            foreach (int studentCourseId in studentCourseIds)
            {
                result = _chuxinWorkFlow.SingleRemoveCourse(studentCourseId);
                if(result != "200")
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 添加放假日期 POST api/coursearrange/addholidays
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string AddHolidays([FromBody] List<SysHoliday> holidays)
        {
            string result = string.Empty;

            foreach(var holiday in holidays)
            {
                result = _chuxinWorkFlow.AddHoliday(holiday);
            }
            
            return result;
        }

        /// <summary>
        /// 删除放假日期 DELETE api/coursearrange/removeholiday
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string RemoveHoliday(string strDay)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveHoliday(strDay);
            return result;
        }
    }   
}