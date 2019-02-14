using System.Collections.Generic;
using ChuXinEdu.CMS.Server.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Model;
using System;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public ActivityController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }        

        // /// <summary>
        // /// 获取活动列表 GET api/comment
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet("{studentCode}")]
        // public IEnumerable<SysActivity> GetActivityList(string studentCode)
        // {
        //     IEnumerable<SysActivity> comments = _chuxinQuery.GetCourseComments(studentCode);
        //     return comments;
        // }

        // POST api/comment
        [HttpPost]
        public string Post([FromBody] StudentCourseComment newComment)
        {
            string result = string.Empty;
            newComment.CreateTime = DateTime.Now;

            string teacherCode = _chuxinQuery.getTeacherCodeByName(newComment.TeacherName);
            newComment.TeacherCode = teacherCode;
            
            result = _chuxinWorkFlow.AddCourseComment(newComment);

            return result;
        }

        // PUT api/comment/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] StudentCourseComment comment)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.UpdateCourseComment(id, comment);
            return result;
        }

        // DELETE api/comment/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveCourseComment(id);

            return result;
        }
    }
}
