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
        public IEnumerable<SysCoursePackage> Get(string q)
        {
            QUERY_SYS_PACKAGE query = JsonConvert.DeserializeObject<QUERY_SYS_PACKAGE>(q);
            IEnumerable<SysCoursePackage> packageList = _chuxinQuery.GetSysCoursePackageList(query);
            return packageList;
        }

        // GET api/coursepackage/5
        [HttpGet("{id}")]
        public bool Get(int id)
        {
            return _chuxinQuery.isPackageUsed(id);
        }

        // POST api/coursepackage
        [HttpPost]
        public string Post([FromBody] SysCoursePackage newPackage)
        {
            string result = string.Empty;
            newPackage.PackageCreateTime = DateTime.Now;
            newPackage.PackageCode = TableCodeHelper.GenerateCode("sys_course_package", "package_code", DateTime.Now);

            result = _chuxinWorkFlow.AddSysCoursePackage(newPackage);

            return result;
        }

        // PUT api/coursepackage/5
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
