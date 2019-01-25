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
using ChuXinEdu.CMS.Server.Utils;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class ArrangeTemplateController : ControllerBase
    {
        private readonly IConfigQuery _configQuery;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkflow;

        public ArrangeTemplateController(IChuXinQuery chuxinQuery, IConfigQuery configQuery, IChuXinWorkFlow chuxinWorkflow)
        {
            _chuxinQuery = chuxinQuery;
            _configQuery = configQuery;
            _chuxinWorkflow = chuxinWorkflow;
        }

        /// <summary>
        /// 获取排课模板列表 list GET api/arrangetemplate/getarrangetemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SysCourseArrangeTemplate> GetArrangeTemplates()
        {
            return _chuxinQuery.GetSysArrangeTemplates();
        }

        /// <summary>
        /// 获取排课模板详细 list GET api/arrangetemplate/getarrangetemplatedetail
        /// </summary>
        /// <returns></returns>
        [HttpGet("{templateCode}")]
        public IEnumerable<SysCourseArrangeTemplateDetail> GetArrangeTemplateDetail(string templateCode)
        {
            return _chuxinQuery.GetArrangeTemplateDetail(templateCode);
        }

        /// <summary>
        /// 添加新模板 POST api/arrangetemplate/addnewtemplate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string AddNewTemplate([FromBody] dynamic template)
        {
            string result = string.Empty;
            string templateName = template.templateName.ToString();
            string templateEnabled = template.templateEnabled.ToString();
            string templateCode = TableCodeHelper.GenerateCode("sys_course_arrange_template", "arrange_template_code", DateTime.Now);

            List<SysCourseArrangeTemplateDetail> details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SysCourseArrangeTemplateDetail>>(Convert.ToString(template.details));

            result = _chuxinWorkflow.AddArrangeTemplate(templateCode, templateName, templateEnabled, details);
            return result;
        }

        /// <summary>
        /// 添加新模板 POST api/arrangetemplate/updatetemplate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateTemplate([FromBody] dynamic template)
        {
            string result = string.Empty;
            string templateCode = template.templateCode.ToString();
            string templateName = template.templateName.ToString();
            string templateEnabled = template.templateEnabled.ToString();

            List<SysCourseArrangeTemplateDetail> details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SysCourseArrangeTemplateDetail>>(Convert.ToString(template.details));

            result = _chuxinWorkflow.UpdateArrangeTemplate(templateCode, templateName, templateEnabled, details);
            return result;
        }

        /// <summary>
        /// 添加新模板 Delete api/arrangetemplate/deltemplate
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{templateCode}")]
        public string DelTemplate(string templateCode)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.RemoveArrangeTemplate(templateCode);
            return result;
        }
    }
}