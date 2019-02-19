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
using Newtonsoft.Json.Serialization;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
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

        // GET api/activity/getlist
        [HttpGet]
        public ActionResult<string> GetList(string q)
        {
            int totalCount = 0;
            QUERY_SYS_ACTIVITY query = JsonConvert.DeserializeObject<QUERY_SYS_ACTIVITY>(q);
            List<SysActivity> activityList = _chuxinQuery.GetActivityList(query, out totalCount);

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd"
            };
            
            return new JsonResult(new {
                TotalCount = totalCount,
                Data = activityList
            }, settings);
        }

        // GET api/activity/getdetail
        [HttpGet("{activityId}")]
        public SysActivity GetDetail(int activityId)
        {
            return _chuxinQuery.GetActivityById(activityId);
        }

        // GET api/activity/getstudents
        [HttpGet("{activityId}")]
        public List<StudentActivity> GetStudents(int activityId)
        {
            return _chuxinQuery.GetStudentByActivity(activityId);
        }

        // POST api/activity/saveactivity
        [HttpPost("{activityId}")]
        public ActionResult<string> SaveActivity(int activityId, [FromBody] SysActivity activity)
        {
            string result = string.Empty;
            int id = 0;
            
            if(activityId == 0)
            {
                result = _chuxinWorkFlow.AddNewActivity(activity, out id);
            }
            else
            {
                result = _chuxinWorkFlow.UpdateActivity(activityId, activity);
                id = activityId;
            }

            return new JsonResult(new { code = result, id = id });
        }

        // POST api/activity/savestudents
        [HttpPost("{activityId}")]
        public ActionResult<string> SaveStudents(int activityId, [FromBody] List<StudentActivity> saList)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.SaveStudent2Activity(activityId, saList);

            return result;
        }

        // DELETE api/activity/delactivity
        [HttpDelete("{id}")]
        public string DelActivity(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveActivity(id);

            return result;
        }
    }
}
