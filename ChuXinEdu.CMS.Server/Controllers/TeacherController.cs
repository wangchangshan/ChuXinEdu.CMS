using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Data;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, ILogger<TeacherController> logger)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _logger = logger;
        }

        // GET api/teacher/getteacherlist
        [HttpGet]
        public IEnumerable<Teacher> GetTeacherList(string q)
        {
            QUERY_TEACHER query = JsonConvert.DeserializeObject<QUERY_TEACHER>(q);
            IEnumerable<Teacher> teacherList = _chuxinQuery.GetTeacherList(query);
            return teacherList;
        }

        // GET api/teacher/getcourseteacherlist
        [HttpGet]
        public ActionResult<string> GetCourseTeacherList()
        {
            string resultJson = string.Empty;
            DataTable dt = _chuxinQuery.GetAllCourseRoleTeachers();
            if (dt != null)
            {
                resultJson = JsonConvert.SerializeObject(dt);
            }
            return resultJson;
        }

        // GET api/teacher/getteacherwithrole
        [HttpGet("{roleCode}")]
        public ActionResult<string> GetTeacherWithRole(string roleCode)
        {
            string resultJson = string.Empty;
            DataTable dt = _chuxinQuery.GetTeacherListWithRole(roleCode);
            if (dt != null)
            {
                resultJson = JsonConvert.SerializeObject(dt);
            }
            return resultJson;
        }

        // GET api/teacher/getteacherbase
        [HttpGet("{teacherCode}")]
        public Teacher GetTeacherBase(string teacherCode)
        {
            Teacher teacher = _chuxinQuery.GetTeacher(teacherCode);
            if (!String.IsNullOrEmpty(teacher.TeacherAvatarPath))
            {
                int id = teacher.Id;
                string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
                teacher.TeacherAvatarPath = accessUrlHost + "api/upload/getimage?id=" + id + "&type=avatar-t&rnd=" + System.Guid.NewGuid().ToString("N");
            }
            return teacher;
        }

        // GET api/teacher/getcourselist
        [HttpGet("{teacherCode}")]
        public ActionResult<string> getCourseList(string teacherCode, int pageIndex, int pageSize, string q)
        {
            QUERY_TEACHER_COURSE query = JsonConvert.DeserializeObject<QUERY_TEACHER_COURSE>(q);
            int totalCount = 0;
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetTeacherCourseList(teacherCode, pageIndex, pageSize, query, out totalCount);

            dynamic obj = new
            {
                TotalCount = totalCount,
                CourseList = courseList
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string resultJson = JsonConvert.SerializeObject(obj, settings);

            return resultJson;
        }

        /// <summary>
        /// [导出教师销课列表] 教师销课列表 GET api/teacher/getcourselist2export
        /// </summary>
        /// <returns></returns>
        [HttpGet("{teacherCode}")]
        public IEnumerable<StudentCourseList> GetCourseList2Export(string teacherCode, string q)
        {
            QUERY_TEACHER_COURSE query = JsonConvert.DeserializeObject<QUERY_TEACHER_COURSE>(q);
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetTeacherCourseList2Export(teacherCode, query);
            return courseList;
        }

        // POST api/teacher/postnewteacher
        [HttpPost]
        public string PostNewTeacher([FromBody] Teacher teacher)
        {
            string result = string.Empty;
            teacher.TeacherCode = TableCodeHelper.GenerateCode("teacher", "teacher_code", DateTime.Now);

            result = _chuxinWorkFlow.AddTeacher(teacher);

            return result;
        }

        // PUT api/teacher/updateteacher
        [HttpPut("{id}")]
        public string UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.UpdateTeacher(id, teacher);
            return result;
        }

        // GET api/teacher/getwxkey
        [HttpGet("{teacherCode}")]
        public ActionResult<string> GetWxkey(string teacherCode)
        {
            string wxKey = string.Empty;
            Teacher teacher = _chuxinQuery.GetTeacher(teacherCode);
            if(String.IsNullOrEmpty(teacher.TeacherWxkey))
            {
                wxKey = Guid.NewGuid().ToString("N").Substring(0, 8);
                string result = _chuxinWorkFlow.UpdateTeacherWxkey(teacherCode, wxKey);
                if(result != "1200")
                {
                    wxKey = "授权码生成错误，请联系管理员。";
                }
            }
            else
            {
                wxKey = teacher.TeacherWxkey;
            }
            return wxKey;
        }
    }
}