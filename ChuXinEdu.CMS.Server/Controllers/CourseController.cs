using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Data;
using System.Net.Http;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public CourseController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _hostingEnvironment = hostingEnvironment;
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
        /// 获取待签到数目 GET api/course/gettorecordcount
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public int GetToRecordCount()
        {
            int count = _chuxinQuery.GetCoursesToSignInCount();
            return count;
        }

        /// <summary>
        /// 获取课时套餐将要结束（5节课）并且没有报新套餐的学生 GET api/course/gettofinish
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public ActionResult<string> GetToFinish()
        {
            // 只显示 当前学生正常在学的，课时数1到5节的学生。（课时数为0，并且没有其他可用套餐，则学生状态会在签到的时候修改为03 结束未续费）
            DataTable dt = _chuxinQuery.GetCourseToFinishList();
            string resultJson = JsonConvert.SerializeObject(dt);        
            return resultJson;   
        }

        /// <summary>
        /// 获取课程作品 GET api/course/getcourseartwork
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<ART_WORK_R_LIST> GetCourseArtwork(int courseId)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<StudentArtwork, ART_WORK_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            IEnumerable<StudentArtwork> artworks = _chuxinQuery.GetArkworkByCourse(courseId);

            List<ART_WORK_R_LIST> artWorkList = new List<ART_WORK_R_LIST>();
            ART_WORK_R_LIST  aw = null;

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var artwork in artworks)
            {
                aw = mapper.Map<StudentArtwork, ART_WORK_R_LIST>(artwork);
                aw.ShowURL = accessUrlHost + "api/upload/getimage?id=" + artwork.ArtworkId + "&type=artwork";

                artWorkList.Add(aw);
            }

            return artWorkList;
        }

        /// <summary>
        /// 提交课程补录信息 POST api/course/coursesupplement
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CourseSupplement([FromBody] List<StudentCourseList> couseList)
        {
            string result = _chuxinWorkFlow.SupplementHistoryCourse(couseList);
            return result;
        }

        /// <summary>
        /// 签到 PUT api/course/putsignin
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutSignIn(CL_U_SIGN_IN course)
        {
            string result = _chuxinWorkFlow.SignInSingle(course);

            return result;
        }

        /// <summary>
        /// 批量签到 PUT api/course/putsigninbatch
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutSignInBatch([FromBody] List<CL_U_SIGN_IN> courseList)
        {
            string result = _chuxinWorkFlow.SignInBatch(courseList);

            return result;
        }

        /// <summary>
        /// 确定补充上传作品 PUT api/course/artworksupplement
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string ArtworkSupplement(CL_U_SIGN_IN course)
        {
            string result = _chuxinWorkFlow.SupplementArtWork(course);

            return result;
        }
    }   
}