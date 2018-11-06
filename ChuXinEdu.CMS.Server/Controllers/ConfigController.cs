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

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigQuery _configQuery;
        private readonly IChuXinQuery _chuxinQuery;

        public ConfigController(IChuXinQuery chuxinQuery, IConfigQuery configQuery)
        {
            _chuxinQuery = chuxinQuery;
            _configQuery = configQuery;
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
    }
}