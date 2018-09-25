using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {        
        private readonly IConfigQuery _configQuery;
        public ConfigController(IConfigQuery configQuery)
        {
            _configQuery = configQuery;
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
        public ActionResult<IEnumerable<DIC_R_KEY_VALUE>> GetCoursePackage(string typeCode)
        {
            return _configQuery.GetDicByCode(typeCode).ToList();
        }
    }   
}