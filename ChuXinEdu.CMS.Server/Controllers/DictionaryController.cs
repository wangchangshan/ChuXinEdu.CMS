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
using System.Data;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IConfigQuery _configQuery;
        private readonly IChuXinWorkFlow _chuxinWorkflow;

        public DictionaryController(IConfigQuery configQuery, IChuXinWorkFlow chuxinWorkflow)
        {
            _configQuery = configQuery;
            _chuxinWorkflow = chuxinWorkflow;
        }


        /// <summary>
        /// [配置] 获取所有字典 GET api/dictionary/getdictionarys
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SysDictionary> GetDictionarys()
        {
            return _configQuery.GetDictionarys();
        }

        /// <summary>
        /// 添加新模板 POST api/dictionary/addnewdic
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string AddNewDic([FromBody] List<SysDictionary> dicList)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.AddNewDic(dicList);
            return result;
        }
    }
}