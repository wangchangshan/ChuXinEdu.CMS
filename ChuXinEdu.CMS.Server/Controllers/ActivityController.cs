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

        // GET api/activity
        [HttpGet]
        public ActionResult<string> Get(string q)
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

        // GET api/activity/1
        [HttpGet("{activityId}")]
        public SysActivity Get(int activityId)
        {
            return _chuxinQuery.GetActivityById(activityId);
        }

        // POST api/activity
        [HttpPost("{activityId}")]
        public ActionResult<string> Post(int activityId, [FromBody] SysActivity activity)
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

        // DELETE api/activity/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveActivity(id);

            return result;
        }
    }
}
