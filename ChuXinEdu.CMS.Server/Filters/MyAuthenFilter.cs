using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 身份验证过滤器
    public class MyAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            string token = filterContext.HttpContext.Request.Headers["token"] + "";
            string loginCode = filterContext.HttpContext.Request.Headers["logincode"] + "";
            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginCode))
            {
                filterContext.Result = new JsonResult(new { code = "1401" });
                return;
            }
            else
            {
                string ip = filterContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); 
                
                loginCode = HttpUtility.UrlDecode(loginCode);
                using (BaseContext context = new BaseContext())
                {
                    var sysUser = context.SysUser.Where(u => u.LoginCode == loginCode
                                                            && u.Token == token
                                                            && u.LastLoginIP == ip
                                                            && u.TokenExpireTime > DateTime.Now)
                                                .FirstOrDefault();
                    if(sysUser != null)
                    {  
                        string strExpireMinu = CustomConfig.GetSetting("UserExpireTime");
                        if (string.IsNullOrEmpty(strExpireMinu))
                        {
                            strExpireMinu = "30";
                        }
                        int expireMinu = Int32.Parse(strExpireMinu);
                        sysUser.TokenExpireTime = DateTime.Now.AddMinutes(expireMinu);
                        context.SaveChanges();
                    }
                    else 
                    {
                        filterContext.Result = new JsonResult(new { code = "1401" });
                    }
                }
            }    
        }
    }
}