using System.Collections.Generic;
using ChuXinEdu.CMS.Server.BLL;
using Microsoft.AspNetCore.Mvc;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePackageController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public CoursePackageController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        // GET api/coursepackage
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/coursepackage/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/coursepackage
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/coursepackage/5
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
