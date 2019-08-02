using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class StudentTempController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkflow;

        public StudentTempController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkflow)
        {
            _chuxinQuery = chuxinQuery;    
            _chuxinWorkflow = chuxinWorkflow;
        }

        /// <summary>
        /// [学生列表] 获取所有学生list GET api/studenttemp/getstudentlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetStudentList(int pageIndex, int pageSize, string q)
        {
            int totalCount = 0;
            QUERY_STUDENT_TEMP query = JsonConvert.DeserializeObject<QUERY_STUDENT_TEMP>(q);  

            IEnumerable<StudentTemp> students = _chuxinQuery.GetTempStudentList(pageIndex, pageSize, query, out totalCount);

            dynamic obj = new {
                TotalCount = totalCount,
                StudentList = students
            };

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string resultJson = JsonConvert.SerializeObject(obj,settings);

            return resultJson;
        }

        /// <summary>
        /// 添加试听学生 PUT api/studenttemp/addstudent
        /// </summary>
        /// <returns>studentCode</returns>
        [HttpPost]
        public string AddStudent([FromBody] StudentTemp student)
        {
            string studentCode = TableCodeHelper.GenerateCode("student", "student_code", DateTime.Now);
            student.StudentCode = studentCode;
            student.CreateTime = DateTime.Now;
            student.Result = "待定";
            string result = _chuxinWorkflow.AddTempStudent(student);
            
            return result;
        }

        /// <summary>
        /// 更新试听学生基本信息 PUT api/studenttemp/updatestudent
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public string UpdateStudent(int id, [FromBody] StudentTemp student)
        {
            string result = _chuxinWorkflow.UpdateTempStudent(id, student);
            return result;
        }

        /// <summary>
        /// 删除试听学生 DELETE api/studenttemp/removestudent
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public string RemoveStudent(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.RemoveTempStudent(id);
            return result;
        }

        /// <summary>
        /// 试听成功 PUT api/studenttemp/trialsuccess
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public string TrialSuccess(int id)
        {
            string result = _chuxinWorkflow.TempStudentTrialSuccess(id);
            return result;
        }

        /// <summary>
        /// 试听失败 PUT api/studenttemp/trialfail
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public string TrialFail(int id)
        {
            string result = _chuxinWorkflow.TempStudentTrialFail(id);
            return result;
        }
    }   
}