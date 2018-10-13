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

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
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

        // GET api/teacher
        [HttpGet]
        public IEnumerable<Teacher> Get()
        {
            IEnumerable<Teacher> teacherList = _chuxinQuery.GetTeacherList();
            return teacherList;
        }

        // POST api/teacher
        [HttpPost]
        public string Post([FromBody] Teacher teacher)
        {
            string result = string.Empty;
            teacher.TeacherCode = TableCodeHelper.GenerateCode("teacher", "teacher_code");

            result = _chuxinWorkFlow.AddTeacher(teacher);

            return result;
        }

        // PUT api/teacher/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] SysCoursePackage package)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.UpdateSysCoursePackage(id, package);
            return result;
        }

        // DELETE api/coursepackage/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveSysCoursePackage(id);

            return result;
        }
    }   
}