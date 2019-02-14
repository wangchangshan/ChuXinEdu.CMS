using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // 返回的context.Result.Value为string
        [HttpGet("{id}")]
        public ActionResult<string> test1(int id)
        {
            dynamic obj = new {
                code = "1200",
                data = "data",
            };
            // string to object
            //OverAll overAll = JsonConvert.DeserializeObject<OverAll>(info);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string resultJson = JsonConvert.SerializeObject(obj,settings);          
            return resultJson;
        }

        //返回的context.Result.Value为jsonobject
        [HttpGet("{id}")]
        public ActionResult<JsonResult> test2(int id)
        {            
            return new JsonResult(new {
                code = "1200",
                data = "data",
            });
        }

        [HttpGet("{id}")]
        public ActionResult<JsonResult> test3(int id)
        {            
            DataTable dt = new DataTable();
            string result = JsonConvert.SerializeObject(dt);
            return new JsonResult(result);
        }

        [HttpGet("{id}")]
        public ActionResult<string> test4(int id)
        {            
            DataTable dt = new DataTable();
            string result = JsonConvert.SerializeObject(dt);
            return result;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
