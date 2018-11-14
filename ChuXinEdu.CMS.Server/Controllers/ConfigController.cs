using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigQuery _configQuery;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkflow;

        public ConfigController(IChuXinQuery chuxinQuery, IConfigQuery configQuery, IChuXinWorkFlow chuxinWorkflow)
        {
            _chuxinQuery = chuxinQuery;
            _configQuery = configQuery;
            _chuxinWorkflow = chuxinWorkflow;
        }

        /// <summary>
        /// [配置] 获取配置键值对 GET api/config/getdics
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetDics(string codes)
        {
            string[] arrCodes = codes.Split(",");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (string dicCode in arrCodes)
            {
                var dicList = _configQuery.GetDicByCode(dicCode).ToList();
                var strJson = JsonConvert.SerializeObject(dicList, settings);
                if(dicList != null)
                {
                    sb.AppendFormat("\"{0}\": {1},", dicCode, strJson);
                }
            }
            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// [配置] 获取配置键值对 GET api/config/getdicbycode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<DIC_R_KEY_VALUE>> GetDicByCode(string typeCode)
        {
            return _configQuery.GetDicByCode(typeCode).ToList();
        }

        /// <summary>
        /// [配置] 获取课程套餐 GET api/config/getcoursepackage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DIC_R_PACKAGE> GetCoursePackage()
        {
            var categoryList = _configQuery.GetDicByCode("course_category").ToList();

            List<DIC_R_PACKAGE> coursePackage = new List<DIC_R_PACKAGE>();
            foreach (var item in categoryList)
            {
                DIC_R_PACKAGE package = new DIC_R_PACKAGE();
                package.Label = item.Label;
                package.Value = item.Value;
                package.children = _configQuery.GetSysCoursePackage(item.Value);

                coursePackage.Add(package);
            }

            return coursePackage;
        }

        /// <summary>
        /// 获取收费教师键值对 list GET api/config/getfinancer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DIC_R_KEY_VALUE> GetFinancer()
        {
            return _chuxinQuery.GetTeacherToCharge();
        }

        /// <summary>
        /// 获取系统角色列表 list GET api/config/getroles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SysDictionary> GetRoles()
        {
            return _chuxinQuery.GetSysRoles();
        }

        /// <summary>
        /// 获取排课模板列表 list GET api/config/getarrangetemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SysCourseArrangeTemplate> GetArrangeTemplates()
        {
            return _chuxinQuery.GetSysArrangeTemplates();
        }

        // POST api/config/addteacherrole
        [HttpPost("{roleCode}")]
        public string AddTeacherRole(string roleCode, [FromBody] List<string> teacherCodes)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.AddTeacherRole(roleCode, teacherCodes);
            return result;
        }

        // POST api/config/removeteacherrole  使用post传递教师编码列表， 也可以用delete,列表将会放到url
        [HttpPost("{roleCode}")]
        public string RemoveTeacherRole(string roleCode, [FromBody] List<string> teacherCodes)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.RemoveTeacherRole(roleCode, teacherCodes);
            return result;
        }
    }
}