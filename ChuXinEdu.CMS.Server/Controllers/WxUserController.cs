using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;
using System.Net.Http;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class WxUserController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public WxUserController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取微信小程序登陆状态 GET api/wxuser/getloginstate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetLoginState(string code)
        {
            string openId = string.Empty;
            string sessionKey = string.Empty;
            string stateCode = string.Empty;

            // 1. 获取当前用户的openid
            string appid = "wxbc88d5a1f9bda2ec";
            string appsecret = "4a70f1859bc22f0a5caaf7e771bad42c";

            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.weixin.qq.com/sns/jscode2session?appid="+appid+"&secret="+appsecret+"&js_code="+code+"&grant_type=authorization_code";
                HttpResponseMessage response = client.GetAsync(url).Result;
                var jsTicket = response.Content.ReadAsStringAsync().Result;
                var ticket = JsonConvert.DeserializeObject<dynamic>(jsTicket);
                openId = ticket.openid;
                sessionKey = ticket.session_key;
            }

            // 2. 判断当前openid是否已经存在数据库中
            SysWxUser wxUser = _chuxinQuery.GetWxUserByOpenId(openId);
            if(wxUser == null)
            {
                //告知小程序，当前用户没有登录过
                stateCode = "404";
            }
            else
            {
                stateCode = "200";
            }

            dynamic obj = new {
                stateCode = stateCode,
                openId = openId,
                sessionKey = sessionKey
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string resultJson = JsonConvert.SerializeObject(obj, settings);          
            return resultJson;
        }
    }
}