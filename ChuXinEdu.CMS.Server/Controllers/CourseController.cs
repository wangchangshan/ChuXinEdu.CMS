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
using System.Web;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public CourseController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取学生排课列表 GET api/course/getarrangedcourselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod)
        {
            IEnumerable<Simplify_StudentCourseList> courseList = _chuxinQuery.GetArrangedCourseList(studentCode, dayCode, coursePeriod);
            return courseList;
        }

        /// <summary>
        /// 获取学生待签到列表 GET api/course/getattendancelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<StudentCourseList> GetAttendanceList()
        {
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetCoursesToSignIn();
            return courseList;
        }

        /// <summary>
        /// 签到 PUT api/course/putsigninsingle
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutSignInBatch(List<CL_U_SIGN_IN> courseList)
        {
            string result = _chuxinWorkFlow.SignInBatch(courseList);

            return result;
        }


        /// <summary>
        /// 签到 上传作品 POST api/course/uploadartwork
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadArtwork()
        {
            string result = string.Empty;
            string coruseId = HttpContext.Request.Form["course_id"];
            //HttpFileCollection df =  HttpContext.Request.Form.Files;
            return result;
        }
    }   
}