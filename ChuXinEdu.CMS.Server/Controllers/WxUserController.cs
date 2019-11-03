using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
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
            string stateCode = string.Empty;
            string sKey = string.Empty;
            string innerPersonCode = string.Empty;
            string innerPersonName = string.Empty;

            // 1. 获取当前用户的openid
            WXTicket ticket = GetWxTicket(code);

            // 2. 判断当前openid是否已经存在数据库中
            SysWxUser wxUser = _chuxinQuery.GetWxUserByOpenId(ticket.OpenId);
            if (wxUser == null)
            {
                //告知小程序，当前用户没有登录过
                stateCode = "1404";
            }
            else
            {
                stateCode = "1200";
                // 生成登陆标识态
                sKey = wxUser.wxUserType + Cryptor.Encrypt(ticket.SessionKey);
                innerPersonCode = wxUser.InnerPersonCode;
                innerPersonName = wxUser.InnerPersonName;
                _chuxinWorkFlow.UpdateWxSKey(ticket.OpenId, sKey);
            }

            dynamic obj = new
            {
                stateCode = stateCode,
                sessionKey = sKey,
                innerPersonCode = innerPersonCode,
                innerPersonName = innerPersonName
            };
            string resultJson = JsonConvert.SerializeObject(obj);
            return resultJson;
        }

        /// <summary>
        /// 获取微信小程序登陆用户信息 GET api/wxuser/getwxuserinfo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetWxUserInfo(string sKey)
        {
            string stateCode = string.Empty;
            string innerPersonCode = string.Empty;
            string innerPersonName = string.Empty;

            SysWxUser wxUser = _chuxinQuery.GetWxUserBySKey(sKey);
            if (wxUser == null)
            {
                //告知小程序，当前用户没有登录过
                stateCode = "1404";
            }
            else
            {
                stateCode = "1200";
                innerPersonCode = wxUser.InnerPersonCode;
                innerPersonName = wxUser.InnerPersonName;
            }

            dynamic obj = new
            {
                stateCode = stateCode,
                innerPersonCode = innerPersonCode,
                innerPersonName = innerPersonName
            };
            string resultJson = JsonConvert.SerializeObject(obj);
            return resultJson;
        }

        /// <summary>
        /// 微信小程序家长注册登陆 GET api/wxuser/pregister
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> PRegister(string code, string studentCode, string studentName)
        {
            string stateCode = string.Empty;
            WXTicket ticket = new WXTicket();
            string sKey = string.Empty;

            // 1. 判断当前学员是否存在
            bool isExist = _chuxinQuery.IsStudentExist(studentCode, studentName);
            if (isExist)
            {
                // 2. 获取当前用户的openid
                ticket = GetWxTicket(code);
                sKey = "1" + Cryptor.Encrypt(ticket.SessionKey);

                // 3. 数据入库
                stateCode = _chuxinWorkFlow.InsertWxLoginInfo(ticket.OpenId, sKey, studentCode, studentName, "", "1");
            }
            else
            {
                stateCode = "1404"; //找不到学生
            }
            
            
            dynamic obj = new
            {
                stateCode = stateCode,
                sessionKey = sKey
            };
            string resultJson = JsonConvert.SerializeObject(obj);
            return resultJson;
        }

        /// <summary>
        /// 微信小程序教师注册登陆 GET api/wxuser/tregister
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> TRegister(string code, string teacherWxKey)
        {
            string stateCode = string.Empty;
            WXTicket ticket = new WXTicket();
            string sKey = string.Empty;
            string teacherName = string.Empty;

            string teacherCode = _chuxinQuery.GetTeacherCodeByWxKey(teacherWxKey, out teacherName);
            if (!String.IsNullOrEmpty(teacherCode))
            {
                // 2. 获取当前用户的openid
                ticket = GetWxTicket(code);
                sKey = "2" + Cryptor.Encrypt(ticket.SessionKey);

                // 3. 数据入库
                stateCode = _chuxinWorkFlow.InsertWxLoginInfo(ticket.OpenId, sKey, teacherCode, teacherName, teacherWxKey, "2");
            }
            else
            {
                stateCode = "1404"; //找不到教师，即注册码错误
            }
            
            dynamic obj = new
            {
                stateCode = stateCode,
                sessionKey = sKey,
                teacherCode = teacherCode,
                teacherName = teacherName
            };
            string resultJson = JsonConvert.SerializeObject(obj);
            return resultJson;
        }

        public WXTicket GetWxTicket(string code)
        {
            WXTicket wxticket = new WXTicket();
            string appid = "wxbc88d5a1f9bda2ec";
            string appsecret = "4a70f1859bc22f0a5caaf7e771bad42c";

            // 1. 获取当前用户的openid
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + appsecret + "&js_code=" + code + "&grant_type=authorization_code";
                HttpResponseMessage response = client.GetAsync(url).Result;
                var jsTicket = response.Content.ReadAsStringAsync().Result;
                var ticket = JsonConvert.DeserializeObject<dynamic>(jsTicket);
                wxticket.OpenId = ticket.openid;
                wxticket.SessionKey = ticket.session_key;
            }

            return wxticket;
        }
    }

    public class WXTicket{
        public string OpenId { get; set; }
        public string SessionKey { get; set; }
    }
}