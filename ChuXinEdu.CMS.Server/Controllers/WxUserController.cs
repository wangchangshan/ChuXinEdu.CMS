using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using System.Net.Http;
using System.Data;
using ChuXinEdu.CMS.Server.ViewModel;
using System.Collections.Generic;

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
            WX_MINE_OVERVIEW overView = null;

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
                overView = GetOverview(wxUser.wxUserType, innerPersonCode);
            }

            dynamic obj = new
            {
                stateCode = stateCode,
                sessionKey = sKey,
                innerPersonCode = innerPersonCode,
                innerPersonName = innerPersonName,
                overView = overView
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
            WX_MINE_OVERVIEW overView = null;

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
                overView = GetOverview(wxUser.wxUserType, innerPersonCode);
            }

            dynamic obj = new
            {
                stateCode = stateCode,
                innerPersonCode = innerPersonCode,
                innerPersonName = innerPersonName,
                overView = overView
            };
            string resultJson = JsonConvert.SerializeObject(obj);
            return resultJson;
        }

        private WX_MINE_OVERVIEW GetOverview(string userType, string personCode)
        {
            WX_MINE_OVERVIEW overView = null;
            switch (userType)
            {
                case "1":
                    // 家长学员
                    Student student = _chuxinQuery.GetStudentByCode(personCode);
                    if (student != null)
                    {
                        DataTable dtRestCourseCount = _chuxinQuery.GetRestCourseCountByCategorty(personCode);
                        foreach (DataRow dr in dtRestCourseCount.Rows)
                        {
                            dr["course_category_name"] = dr["course_category_name"].ToString() + "剩余课时";
                        }
                        string strCourseInfo = JsonConvert.SerializeObject(dtRestCourseCount);
                        int artWorkCount = _chuxinQuery.GetStudentArkworkCount(personCode);
                        overView = new WX_MINE_OVERVIEW
                        {
                            studentBirthday = student.StudentBirthday,
                            studentSex = student.StudentSex,
                            studentPhone = student.StudentPhone,
                            studentAddress = student.StudentAddress,
                            studentArtworkCount = artWorkCount,
                            studentCourseOverview = strCourseInfo
                        };
                    }
                    break;
                case "2":
                    // 教师
                    // int studentCount = 1; // 测试
                    int studentCount = _chuxinQuery.GetActiveStudentCount();
                    int todayCourseCount = _chuxinQuery.GetTodayCourseCount();
                    DataTable dt = _chuxinQuery.GetBirthdayIn7Days();
                    // int birthCount = 0;//测试
                    int birthCount = dt.Rows.Count;
                    dt = _chuxinQuery.GetExpirationStudents();
                    // int expirationCount = 0;//测试
                    int expirationCount = dt.Rows.Count;

                    overView = new WX_MINE_OVERVIEW
                    {
                        tStudentCount = studentCount,
                        tTodayCourseCount = todayCourseCount,
                        tStudentBirthCount = birthCount,
                        tExpirationCount = expirationCount
                    };
                    break;
                default:
                    break;
            }

            return overView;
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
            string appid = CustomConfig.GetSetting("WeiXinAppId");
            string appsecret = CustomConfig.GetSetting("WeiXinAppSecret");

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

    public class WXTicket
    {
        public string OpenId { get; set; }
        public string SessionKey { get; set; }
    }
}