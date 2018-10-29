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
using ChuXinEdu.CMS.Server.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public TeacherController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        // GET api/teacher/getteacherlist
        [HttpGet]
        public IEnumerable<Teacher> GetTeacherList(string q)
        {
            QUERY_TEACHER query = JsonConvert.DeserializeObject<QUERY_TEACHER>(q);
            IEnumerable<Teacher> teacherList = _chuxinQuery.GetTeacherList(query);
            return teacherList;
        }

        // GET api/teacher/getteacherbase
        [HttpGet("{teacherCode}")]
        public Teacher GetTeacherBase(string teacherCode)
        {
            Teacher  teacher = _chuxinQuery.GetTeacher(teacherCode);
            if(!String.IsNullOrEmpty(teacher.TeacherAvatarPath))
            {
                int id = teacher.Id;
                teacher.TeacherAvatarPath = "http://localhost:8080/api/course/getimage?id=" + id + "&type=avatar-t&rnd="+ System.Guid.NewGuid().ToString("N");
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

            dynamic obj = new {
                TotalCount = totalCount,
                CourseList = courseList
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string reslutJson = JsonConvert.SerializeObject(obj,settings);

            return reslutJson;
        }

        /// <summary>
        /// [导出教师销课列表] 教师销课列表 GET api/teacher/getcourselist2export
        /// </summary>
        /// <returns></returns>
        [HttpGet("{teacherCode}")]
        public IEnumerable<StudentCourseList> GetCourseList2Export(string teacherCode, string q)
        { 
            QUERY_TEACHER_COURSE query = JsonConvert.DeserializeObject<QUERY_TEACHER_COURSE>(q);            
            IEnumerable<StudentCourseList>  courseList = _chuxinQuery.GetTeacherCourseList2Export(teacherCode, query);
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
    }   
}